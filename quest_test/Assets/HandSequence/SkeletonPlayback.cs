using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class SkeletonPlayback : MonoBehaviour, HandSequence.SkeletonHandSequenceProvider, MIDIDevice.MidiDataProvider
{
    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;
    
    [SerializeField]
    private HandSequence _sequence;

    [SerializeField]
    private bool _loop;
    
    private int _currentFrame = 0;
    private int _lastFrame = -1;

    private float _playbackTime;
    private float _startTime;

    private bool _isPlaying = false;
    
    public bool isInitialized { get; private set; }

    public bool PlayMidi;


    public List<HandSequence.SerializableNoteEvent> _midiEventBuffer;

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
        // returning and reseting buffer
        var oldBuffer = _midiEventBuffer;
        _midiEventBuffer = new List<HandSequence.SerializableNoteEvent>();
        return oldBuffer;
    }

    private void StartPlayback()
    {
        Debug.Log("is playing");
        _currentFrame = 0;
        _startTime = Time.time;
        _isPlaying = true;
    }
    private void StopPlayback()
    {
        Debug.Log("Stopped playback");
        _isPlaying = false;
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
                _midiEventBuffer.AddRange(frame.MidiData);
            }

            offset++;
        }
    }
    

    void Start()
    {
        _midiEventBuffer = new List<HandSequence.SerializableNoteEvent>();

        isInitialized = _sequence.hasData();
        if (isInitialized)
        {
            StartPlayback();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartPlayback();
        }

        if (_isPlaying)
        {
            _playbackTime = Time.time - _startTime;
            SetFrameFromTime();
        }
    }
}