using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>

/// </summary>
[Serializable]
public class HandSequence : ScriptableObject
{
     public List<HandFrame> frames;
     public int length;
     public bool static_rec = true;

     public class HandFrame
     {
          public OVRSkeleton.SkeletonPoseData frameData;
          public float time;
     }
}
