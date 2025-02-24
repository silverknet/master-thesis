using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides playback of handsequence.
/// Use by adding this together with a renderer.
/// </summary>
public class SkeletonPlayback : MonoBehaviour, HandSequence.SkeletonHandSequenceProvider
{
    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;
    
    [SerializeField]
    private HandSequence _sequence;

    [SerializeField]
    private bool _loop;
    
    private int _currentFrame = 0;

    private float _playbackTime;
    private float _startTime;

    private bool _isPlaying = false;
    
    public bool isInitialized { get; private set; }

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
        int offset = 0;
        while (true)
        {
            if (_currentFrame + offset >= _sequence.Length)
            {
                if (_loop)
                {
                    // loop around
                    _startTime = Time.time;
                    _currentFrame = 0;
                    _playbackTime = 0.0f;
                    offset = 0;
                }
                else
                {
                    StopPlayback();
                    break;
                }
            }
            
            HandSequence.HandFrame frame = _sequence.frames[_currentFrame + offset];

            if (_playbackTime < frame.time)
            {
                _currentFrame = _currentFrame + (offset-1);
                return;
            }
            offset++;
        }
    }
    

    void Start()
    {
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