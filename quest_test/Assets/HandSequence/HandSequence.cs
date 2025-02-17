using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>

/// </summary>
[Serializable]
public class HandSequence : ScriptableObject
{

     [SerializeField] public List<HandFrame> frames = new List<HandFrame>();
     public int length;
     public bool static_rec = true;

     [Serializable]
     public class HandFrame
     {
          //public OVRSkeleton.SkeletonPoseData frameData;

          public float time;

          public posef RootPose;
          
          public float RootScale;
          
          public quatf[] BoneRotations;
          
          public bool IsDataValid;
          
          public bool IsDataHighConfidence;
          
          public vec3f[] BoneTranslations;
          
          public int SkeletonChangedCount;

          public static explicit operator OVRSkeleton.SkeletonPoseData(HandFrame obj)
          {
               OVRSkeleton.SkeletonPoseData output = new OVRSkeleton.SkeletonPoseData();

               output.RootPose = (OVRPlugin.Posef)obj.RootPose;
               output.RootScale = obj.RootScale;

               output.BoneRotations = new OVRPlugin.Quatf[obj.BoneRotations.Length];
               for (int i = 0; i < obj.BoneRotations.Length; i++)
               {
                    output.BoneRotations[i] = (OVRPlugin.Quatf)obj.BoneRotations[i];
               }

               output.IsDataValid = obj.IsDataValid;
               output.IsDataHighConfidence = obj.IsDataHighConfidence;

               output.BoneTranslations = new OVRPlugin.Vector3f[obj.BoneTranslations.Length];
               for (int i = 0; i < obj.BoneTranslations.Length; i++)
               {
                    output.BoneTranslations[i] = (OVRPlugin.Vector3f)obj.BoneTranslations[i];
               }

               output.SkeletonChangedCount = obj.SkeletonChangedCount;
               
               return output;
          }       
     }

     public string test_print(){
          return frames.Count.ToString();
          return frames[0].IsDataValid.ToString();
     }

     [Serializable]
     public struct quatf{
          public float x;
          public float y; 
          public float z;
          public float w;
          public static explicit operator OVRPlugin.Quatf(quatf obj)
          {
               return new OVRPlugin.Quatf{x = obj.x, y = obj.y, z = obj.z, w = obj.w};
          }
     }
     [Serializable]
     public struct vec3f{
          public float x;
          public float y; 
          public float z;
          public static explicit operator OVRPlugin.Vector3f(vec3f obj)
          {
               return new OVRPlugin.Vector3f{x = obj.x, y = obj.y, z = obj.z};
          }
     }
     [Serializable]
     public struct posef
	{
		public quatf Orientation;
		public vec3f Position;

          public static explicit operator OVRPlugin.Posef(posef obj)
          {
               return new OVRPlugin.Posef{Orientation = (OVRPlugin.Quatf)obj.Orientation, Position = (OVRPlugin.Vector3f)obj.Position};
          }
	}
}
