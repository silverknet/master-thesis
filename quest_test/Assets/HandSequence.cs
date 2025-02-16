using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>

/// </summary>
[Serializable]
public class HandSequence : ScriptableObject
{
     List<HandFrame> data;

     public class HandFrame
     {
          public OVRSkeleton.SkeletonPoseData frameData;
          public float time;
     }
}
