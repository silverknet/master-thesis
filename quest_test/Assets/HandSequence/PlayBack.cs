using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBack : MonoBehaviour, OVRSkeleton.IOVRSkeletonDataProvider
{
    
    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;
    
    [SerializeField]
    private HandSequence _sequence;

    public OVRSkeleton.SkeletonType GetSkeletonType()
    {
        return _skeletonType;
    }
    
    public OVRSkeleton.SkeletonPoseData GetSkeletonPoseData()
    {
        Debug.Log("GetSkeletonPoseData called");
        return (OVRSkeleton.SkeletonPoseData)_sequence.frames[0];
    }

    
    void Start()
    {
        var skeleton = GetComponent<OVRSkeleton>();
        Debug.Log("sequence length: " + _sequence.length);
        Debug.Log("test: " + _sequence.test_print());
        Debug.Log(" * root pose position :" + _sequence.frames[0].RootPose.Position.x);
        Debug.Log(" * valid :" + _sequence.frames[0].IsDataValid);
        Debug.Log(" * high conf:" + _sequence.frames[0].IsDataHighConfidence);

        //skeleton.SetSkeletonType(_skeletonType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
