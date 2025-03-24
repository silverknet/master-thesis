using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

/// <summary>
/// Importer for a OpenXR animated hand sequence, recorded from 
/// 
/// file structure, one line correponds to one frame, all of the below will be on one line
/// rootpose.orientation.x, rootpose.orientation.y, rootpose.orientation.z, rootpose.orientation.w, 
/// rootpose.Position.x, rootpose.Position.y, rootpose.Position.z, 
/// rootScale(float), 
/// [bonerotations(quatf)*26], 
/// isdatavalid(bool), 
/// isdatahighconfidence(bool),
/// [bonetranslatations(quatf)*26],
/// skeletonchangedcount(int)
///
/// </summary>

public class DataRecorder :  MonoBehaviour
{

    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;

    private HandSequence.SkeletonHandSequenceProvider _dataProvider;
    private MIDIDevice.MidiDataProvider _midiDataProvider;

    [SerializeField]
    private string _saveLocation;

    private bool _isRecording;

    private int _currentRecording;

    private bool _hasRecording;

    private List<HandSequence> _handSequenceRecordings;

    public bool recordMidi;
    
    //Time in second where the last recording started
    private float _startTime;
    
    [SerializeField]
    private string _fileName;
    

    private void RecordCurrentFrame()
    {
        HandSequence.HandFrame data = _dataProvider.GetHandFrameData();
        data.time = Time.time - _startTime;
        data.HasMidi = recordMidi;
        if(recordMidi){
            data.MidiData = _midiDataProvider.GetMidiData();
        }
    
        _handSequenceRecordings[_currentRecording].frames.Add(data);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isRecording)
            {
                //start new recording
                _startTime = Time.time;
                _hasRecording = true;
                _handSequenceRecordings.Add(ScriptableObject.CreateInstance<HandSequence>());
                Debug.Log(" * RECORDING STARTED *");
            }
            else
            {
                //Stop recording
                Debug.Log(" * RECORDING STOPPED *");
                _currentRecording += 1;
            }

            _isRecording = !_isRecording;
        }  
        
        if (_isRecording) {
            RecordCurrentFrame();
        }
    }
    void Start()
    {
        _isRecording = false;
        _currentRecording = 0;
        _handSequenceRecordings = new List<HandSequence>();
        
        if (_dataProvider == null)
        {
            var foundDataProvider = SearchSkeletonDataProvider();
            if (foundDataProvider != null)
            {
                _dataProvider = foundDataProvider;
                if (_dataProvider is MonoBehaviour mb)
                {
                    Debug.Log($"Recorder Found IOVRSkeletonDataProvider reference in {mb.name} due to unassigned field.");
                }
            }else{
                Debug.LogWarning("didn't find a data provider for recording" ); 
            }
        }
        if(_midiDataProvider == null && recordMidi){
            var foundDataProvider = SearchMidiDataProvider();
            if (foundDataProvider != null)
            {
                _midiDataProvider = foundDataProvider;
                if (_midiDataProvider is MonoBehaviour mb)
                {
                    Debug.Log($"found mididataprovider");
                }
            }else{
                Debug.LogWarning("didn't find a midi data provider for recording, uncheck recordMidi or add provider"); 
                recordMidi = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("application quit");
        if(_hasRecording) ExportFiles();
    }

    private void ExportFiles()
    {
        GameObject keyboardConfig = GameObject.Find("KeyboardConfiguration");
        if(keyboardConfig != null){
            Debug.Log("found config. now transforming into keyboard space");
            ConfigurePhysicalKeyboard config = keyboardConfig.GetComponent<ConfigurePhysicalKeyboard>();
            Matrix4x4 inverseKeyboardSpaceMatrix = config.getInverseSpaceMatrix();
            foreach (var handSequence in _handSequenceRecordings)
            {
                Debug.Log("applying");
                Debug.Log(inverseKeyboardSpaceMatrix);
                handSequence.applyTransformation(inverseKeyboardSpaceMatrix);
            }
        }

        int nr = 0;
        foreach (var handSequence in _handSequenceRecordings)
        {
            string filename = _fileName + "(" + nr + ")";
            HandSequenceExporter.Export(handSequence, filename, _saveLocation);
            nr++;
        }
    }

    internal HandSequence.SkeletonHandSequenceProvider SearchSkeletonDataProvider()
    {
        var oldProviders = gameObject.GetComponentsInParent<HandSequence.SkeletonHandSequenceProvider>();
        foreach (var dataProvider in oldProviders)
        {
            if (dataProvider.GetSkeletonType() == _skeletonType)
            {
                Debug.Log("Data provider found for Recorder");
                return dataProvider;
            }
        }

        return null;
    }
    internal MIDIDevice.MidiDataProvider SearchMidiDataProvider()
    {
        var oldProviders = gameObject.GetComponentsInParent<MIDIDevice.MidiDataProvider>();
        foreach (var dataProvider in oldProviders)
        {
            return dataProvider;
        }

        return null;
    }

    // public void OnValidate()
    // {
    //     var skeleton = GetComponent<OVRSkeleton>();
    //     if (skeleton != null)
    //     {
    //         if (skeleton.GetSkeletonType() != _skeletonType)
    //         {
    //             MethodInfo setSkeletonTypeMethod = typeof(OVRSkeleton).GetMethod("SetSkeletonType",
    //                 BindingFlags.Instance | BindingFlags.NonPublic); // Access protected method
    //
    //             if (setSkeletonTypeMethod != null)
    //             {
    //                 setSkeletonTypeMethod.Invoke(skeleton, new object[] { _skeletonType });
    //             }
    //             else
    //             {
    //                 Debug.LogError("SetSkeletonType() method not found");
    //             }
    //         }
    //     }
    // }
}
