using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

/// <summary>
/// Provides playback of handsequence.
/// Use by adding this together with a renderer.
/// </summary>
public class SkeletonPlayback : MonoBehaviour, 
HandSequence.SkeletonHandSequenceProvider, 
MIDIDevice.MidiDataProvider, 
KeyboardVisualizer.KeyboardDataProvider
{
    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;
    
    [SerializeField]
    private HandSequence _importSequence;
    private HandSequence _sequence;

    [SerializeField]
    private bool _loop;
    
    private int _currentFrame = 0;

    private float _playbackTime;
    private float _startTime;

    private bool _isPlaying = false;
    public bool IsPlaying
    {
        get { return _isPlaying; }
    }
    private HashSet<int> _notesDown;

    // for keyboard vis
    public HashSet<int> GetNotesDown(){
        return _notesDown;
    }
    public event Action<NoteEvent> OnNoteUpdate;
    
    public bool isInitialized { get; private set; }

    public bool PlayMidi;

    public List<HandSequence.SerializableNoteEvent> _midiEventBuffer;

    private Matrix4x4 _currentKeyboardSpaceMatrix;

    /// <summary>
    /// Will be initialized if it has data
    /// </summary>
    /// <returns></returns>
    public bool IsInitialized()
    {
        return isInitialized;
    }

    public OVRSkeleton.SkeletonType GetSkeletonType()
    {
        return _skeletonType;
    }
    
    public HandSequence.HandFrame GetHandFrameData()
    {
        if (!_isPlaying) return null;
        return _sequence.frames[_currentFrame];
    }

    public List<HandSequence.SerializableNoteEvent> GetMidiData(){
        if (!_isPlaying) return null;
        // returning and reseting buffer
        var oldBuffer = _midiEventBuffer;
        _midiEventBuffer = new List<HandSequence.SerializableNoteEvent>();
        return oldBuffer;
    }

    private void StartPlayback()
    {
        _notesDown = new HashSet<int>();
        GameObject keyboardConfig = GameObject.Find("KeyboardConfiguration");
        if(keyboardConfig != null){
            ConfigurePhysicalKeyboard config = keyboardConfig.GetComponent<ConfigurePhysicalKeyboard>();
            _currentKeyboardSpaceMatrix = config.getSpaceMatrix();
            
            _sequence.applyTransformation(_currentKeyboardSpaceMatrix);
            Debug.Log("Applying transform on start playback");
            Debug.Log(_currentKeyboardSpaceMatrix);
            
        }

        Debug.Log("is playing");
        _currentFrame = 0;
        _startTime = Time.time;
        _isPlaying = true;
    }
    private void StopPlayback()
    {
        Debug.Log("Stopped playback");
        _isPlaying = false;
        _sequence.applyTransformation(_currentKeyboardSpaceMatrix.inverse);
        Debug.Log("Applying transform on stop playback");
         Debug.Log(_currentKeyboardSpaceMatrix);
    }
    // pretty bad but simple algorithm to choose a frame, 
    // just chooses the frame before in time.
    void SetFrameFromTime()
    {
        int offset = 1;
        while (true)
        {   
            // on end of recording
            if (_currentFrame + offset >= _sequence.Length)
            {
                if (_loop)
                {
                    // loop around
                    _startTime = Time.time;
                    _currentFrame = 0;
                    _playbackTime = 0.0f;
                    offset = 1;
                }
                else
                {
                    StopPlayback();
                    break;
                }
            }
            
            HandSequence.HandFrame frame = _sequence.frames[_currentFrame + offset];
            List<HandSequence.SerializableNoteEvent> currentMidiData = new List<HandSequence.SerializableNoteEvent>();
            

            if (_playbackTime < frame.time)
            {
                _currentFrame = _currentFrame + (offset-1);
                return;
            }

            if(PlayMidi){
                if(frame.MidiData.Count != 0){
                    _midiEventBuffer.AddRange(frame.MidiData);

                    //Every frame midi event should just run once, 
                    // TODO Test this, im not sure this method is solid enough
                    foreach(var e in frame.MidiData){
                        OnEventReceived(e.ToNoteEvent());
                    }
                }
            }

            offset++;
        }
    }

    private void OnEventReceived(NoteEvent e)
    {
        var thisNoteEvent = e;
        var number = (int)thisNoteEvent.NoteNumber;
        if(thisNoteEvent.EventType == MidiEventType.NoteOn){
            _notesDown.Add(number);
            OnNoteUpdate?.Invoke((NoteEvent)thisNoteEvent);
        }

        if(thisNoteEvent.EventType == MidiEventType.NoteOff){
            _notesDown.Remove(number);
            OnNoteUpdate?.Invoke((NoteEvent)thisNoteEvent);
        }
    }
    

    void Start()
    {

        _notesDown = new HashSet<int>();
        _sequence = _importSequence.DeepCopy();
        _midiEventBuffer = new List<HandSequence.SerializableNoteEvent>();

        isInitialized = _sequence.hasData();
        /*if (isInitialized)
        {
            StartPlayback();
        }*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(!_isPlaying){
                StartPlayback();
            }else{
                StopPlayback();
            }
        }

        if (_isPlaying)
        {
            _playbackTime = Time.time - _startTime;
            SetFrameFromTime();
        }
    }
}