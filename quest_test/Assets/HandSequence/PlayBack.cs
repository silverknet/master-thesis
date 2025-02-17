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
        Debug.Log("this boy is getting called");
        return _sequence.frames[0].frameData;
    }

    
    
    void Start()
    {
        var skeleton = GetComponent<OVRSkeleton>();
        //skeleton.SetSkeletonType(_skeletonType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
