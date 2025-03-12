using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
public static class OVRHandData
{
    public enum ovrHandEnum {
        Palm, 
        Wrist, 
        ThumbMetacarpal, 
        ThumbProximal, 
        ThumbDistal, 
        ThumbTip,
        IndexMetacarpal, 
        IndexProximal, 
        IndexIntermediate, 
        IndexDistal, 
        IndexTip,
        MiddleMetacarpal, 
        MiddleProximal, 
        MiddleIntermediate, 
        MiddleDistal, 
        MiddleTip,
        RingMetacarpal, 
        RingProximal, 
        RingIntermediate, 
        RingDistal, 
        RingTip,
        LittleMetacarpal, 
        LittleProximal, 
        LittleIntermediate, 
        LittleDistal, 
        LittleTip,
        Invalid
    }

    public  struct JointData
    {
        public ovrHandEnum Parent;
        public ovrHandEnum ID;

        public JointData(ovrHandEnum id, ovrHandEnum parent)
        {
            Parent = parent;
            ID = id;
        }
    }

    public static HashSet<int> hasParent = new HashSet<int> { 3, 4, 5, 7,8,9,10,12,13,14,15,17,18,19,20,22,23,24,25 };
    public static HashSet<int> hasParentWrist = new HashSet<int> {2,6,11,16,21};

    // Use this to go from OpenXR order to OVR order
    public static int GetJointIndex(ovrHandEnum jointID){
        //int i = jointID.ToIndex() - 1;
        int i = (int)jointID;
        return i;
    }
    // inverse of the above
    public static ovrHandEnum GetJointIDfromIndex(int index){
        return (ovrHandEnum)index;
    }

    // This should be the correct order in which the data comes in from OVR
    /*public static List<JointData> jointsOpenXr = new List<JointData>
    {
        new JointData(XRHandJointID.Palm, XRHandJointID.Invalid),
        new JointData(XRHandJointID.Wrist, XRHandJointID.Invalid),
        new JointData(XRHandJointID.ThumbMetacarpal, XRHandJointID.Wrist),
        new JointData(XRHandJointID.ThumbProximal, XRHandJointID.ThumbMetacarpal),
        new JointData(XRHandJointID.ThumbDistal, XRHandJointID.ThumbProximal),
        new JointData(XRHandJointID.ThumbTip, XRHandJointID.ThumbDistal),
        new JointData(XRHandJointID.IndexMetacarpal, XRHandJointID.Wrist),
        new JointData(XRHandJointID.IndexProximal, XRHandJointID.IndexMetacarpal),
        new JointData(XRHandJointID.IndexIntermediate, XRHandJointID.IndexProximal),
        new JointData(XRHandJointID.IndexDistal, XRHandJointID.IndexIntermediate),
        new JointData(XRHandJointID.IndexTip, XRHandJointID.IndexDistal),
        new JointData(XRHandJointID.MiddleMetacarpal, XRHandJointID.Wrist),
        new JointData(XRHandJointID.MiddleProximal, XRHandJointID.MiddleMetacarpal),
        new JointData(XRHandJointID.MiddleIntermediate, XRHandJointID.MiddleProximal),
        new JointData(XRHandJointID.MiddleDistal, XRHandJointID.MiddleIntermediate),
        new JointData(XRHandJointID.MiddleTip, XRHandJointID.MiddleDistal),
        new JointData(XRHandJointID.RingMetacarpal, XRHandJointID.Wrist),
        new JointData(XRHandJointID.RingProximal, XRHandJointID.RingMetacarpal),
        new JointData(XRHandJointID.RingIntermediate, XRHandJointID.RingProximal),
        new JointData(XRHandJointID.RingDistal, XRHandJointID.RingIntermediate),
        new JointData(XRHandJointID.RingTip, XRHandJointID.RingDistal),
        new JointData(XRHandJointID.LittleMetacarpal, XRHandJointID.Wrist),
        new JointData(XRHandJointID.LittleProximal, XRHandJointID.LittleMetacarpal),
        new JointData(XRHandJointID.LittleIntermediate, XRHandJointID.LittleProximal),
        new JointData(XRHandJointID.LittleDistal, XRHandJointID.LittleIntermediate),
        new JointData(XRHandJointID.LittleTip, XRHandJointID.LittleDistal)
    };*/

    public static List<JointData> jointsCustom = new List<JointData>
    {
        new JointData(ovrHandEnum.Palm, ovrHandEnum.Invalid),
        new JointData(ovrHandEnum.Wrist, ovrHandEnum.Invalid),
        new JointData(ovrHandEnum.ThumbMetacarpal, ovrHandEnum.Wrist),
        new JointData(ovrHandEnum.ThumbProximal, ovrHandEnum.ThumbMetacarpal),
        new JointData(ovrHandEnum.ThumbDistal, ovrHandEnum.ThumbProximal),
        new JointData(ovrHandEnum.ThumbTip, ovrHandEnum.ThumbDistal),
        new JointData(ovrHandEnum.IndexMetacarpal, ovrHandEnum.Wrist),
        new JointData(ovrHandEnum.IndexProximal, ovrHandEnum.IndexMetacarpal),
        new JointData(ovrHandEnum.IndexIntermediate, ovrHandEnum.IndexProximal),
        new JointData(ovrHandEnum.IndexDistal, ovrHandEnum.IndexIntermediate),
        new JointData(ovrHandEnum.IndexTip, ovrHandEnum.IndexDistal),
        new JointData(ovrHandEnum.MiddleMetacarpal, ovrHandEnum.Wrist),
        new JointData(ovrHandEnum.MiddleProximal, ovrHandEnum.MiddleMetacarpal),
        new JointData(ovrHandEnum.MiddleIntermediate, ovrHandEnum.MiddleProximal),
        new JointData(ovrHandEnum.MiddleDistal, ovrHandEnum.MiddleIntermediate),
        new JointData(ovrHandEnum.MiddleTip, ovrHandEnum.MiddleDistal),
        new JointData(ovrHandEnum.RingMetacarpal, ovrHandEnum.Wrist),
        new JointData(ovrHandEnum.RingProximal, ovrHandEnum.RingMetacarpal),
        new JointData(ovrHandEnum.RingIntermediate, ovrHandEnum.RingProximal),
        new JointData(ovrHandEnum.RingDistal, ovrHandEnum.RingIntermediate),
        new JointData(ovrHandEnum.RingTip, ovrHandEnum.RingDistal),
        new JointData(ovrHandEnum.LittleMetacarpal, ovrHandEnum.Wrist),
        new JointData(ovrHandEnum.LittleProximal, ovrHandEnum.LittleMetacarpal),
        new JointData(ovrHandEnum.LittleIntermediate, ovrHandEnum.LittleProximal),
        new JointData(ovrHandEnum.LittleDistal, ovrHandEnum.LittleIntermediate),
        new JointData(ovrHandEnum.LittleTip, ovrHandEnum.LittleDistal)
    };

    
}
