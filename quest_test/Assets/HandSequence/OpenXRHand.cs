using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
public static class OpenXRHand
{
    public  struct JointData
    {
        public XRHandJointID Parent;
        public XRHandJointID ID;

        public JointData(XRHandJointID id, XRHandJointID parent)
        {
            Parent = parent;
            ID = id;
        }
    }

    public static List<JointData> joints = new List<JointData>
    {
        new JointData(XRHandJointID.Wrist, XRHandJointID.Invalid),
        new JointData(XRHandJointID.Palm, XRHandJointID.Invalid),
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
    };
}
