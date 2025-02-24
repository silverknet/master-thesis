using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>

/// </summary>
[Serializable]
public class HandSequence : ScriptableObject
{
     
     public interface SkeletonHandSequenceProvider
     {
          OVRSkeleton.SkeletonType GetSkeletonType();
          HandFrame GetHandFrameData();
          bool IsInitialized();
     }

     private OVRSkeleton.SkeletonType _skeletonType;
     public OVRSkeleton.SkeletonType SkeletonType { get; private set; }

     [SerializeField] public List<HandFrame> frames = new List<HandFrame>();
     public int Length { get { return frames.Count; } }

     [Serializable]
     public class HandFrame
     {
          
          public float time;

          public posef RootPose;
          
          public float RootScale;
          
          public Quaternion[] BoneRotations;
          
          public bool IsDataValid;
          
          public bool IsDataHighConfidence;
          
          // This data is not correctly mapped, use: OVRExtensions.FromFlippedZVector3f
          public Vector3[] BoneTranslations;
          
          public int SkeletonChangedCount;

          public static explicit operator OVRSkeleton.SkeletonPoseData(HandFrame obj)
          {
               OVRSkeleton.SkeletonPoseData output = new OVRSkeleton.SkeletonPoseData();

               output.RootPose = (OVRPlugin.Posef)obj.RootPose;
               output.RootScale = obj.RootScale;

               output.BoneRotations = new OVRPlugin.Quatf[obj.BoneRotations.Length];
               for (int i = 0; i < obj.BoneRotations.Length; i++)
               {
                    output.BoneRotations[i] = new OVRPlugin.Quatf
                    {
                         x = obj.BoneRotations[i].x, y = obj.BoneRotations[i].y, z = obj.BoneRotations[i].z,
                         w = obj.BoneRotations[i].w
                    };
               }

               output.IsDataValid = obj.IsDataValid;
               output.IsDataHighConfidence = obj.IsDataHighConfidence;

               output.BoneTranslations = new OVRPlugin.Vector3f[obj.BoneTranslations.Length];
               for (int i = 0; i < obj.BoneTranslations.Length; i++)
               {
                    output.BoneTranslations[i] = new OVRPlugin.Vector3f()
                    {
                         x = obj.BoneTranslations[i].x, y = obj.BoneTranslations[i].y, z = obj.BoneTranslations[i].z
                    };
               }

               output.SkeletonChangedCount = obj.SkeletonChangedCount;
               
               return output;
          }
          public static explicit operator HandFrame(OVRSkeleton.SkeletonPoseData obj)
          {
               HandFrame output = new HandFrame();
               output.RootPose = (posef)obj.RootPose;
               output.RootScale = obj.RootScale;
               
               output.BoneRotations = new Quaternion[obj.BoneRotations.Length];
               for (int i = 0; i < obj.BoneRotations.Length; i++)
               {
                    output.BoneRotations[i] = new Quaternion()
                    {
                         x = obj.BoneRotations[i].x, y = obj.BoneRotations[i].y, z = obj.BoneRotations[i].z,
                         w = obj.BoneRotations[i].w
                    };
               }
               
               output.IsDataValid = obj.IsDataValid;
               output.IsDataHighConfidence = obj.IsDataHighConfidence;

               output.BoneTranslations = new Vector3[obj.BoneTranslations.Length];
               for (int i = 0; i < obj.BoneTranslations.Length; i++)
               {
                    output.BoneTranslations[i] = new Vector3()
                    {
                         x = obj.BoneTranslations[i].x, y = obj.BoneTranslations[i].y, z = obj.BoneTranslations[i].z
                    };
               }

               output.SkeletonChangedCount = obj.SkeletonChangedCount;
               
               return output;
          }

          public override string ToString()
          {
               string logLine = RootPose.ToString();
            
               for(int i = 0; i < BoneRotations.Length; i++){
                    logLine += "," + BoneRotations[i].ToString();
               }

               logLine += "," + (IsDataValid ? "1" : "0"); 
               logLine += "," + (IsDataHighConfidence ? "1" : "0"); 

               for(int i = 0; i < BoneTranslations.Length; i++){
                    logLine += "," + BoneTranslations[i].ToString();
               }

               logLine += "," + SkeletonChangedCount.ToString();
               
               logLine += "," + time.ToString();

               return logLine.Replace(" ", "");
          }
     }

     public bool hasData()
     {
          return frames.Count != 0;
     }

     public int getNumBones()
     {
          return frames[0].BoneTranslations.Length;
     }

     public string test_print(){
          return frames.Count.ToString();
          return frames[0].IsDataValid.ToString();
     }
     // REMOVE WHEN POSSIBLE
     // [Serializable]
     // public struct quatf{
     //      public float x;
     //      public float y; 
     //      public float z;
     //      public float w;
     //      public static explicit operator OVRPlugin.Quatf(quatf obj)
     //      {
     //           return new OVRPlugin.Quatf{x = obj.x, y = obj.y, z = obj.z, w = obj.w};
     //      }
     // }
     // [Serializable]
     // public struct vec3f{
     //      public float x;
     //      public float y; 
     //      public float z;
     //      public static explicit operator OVRPlugin.Vector3f(vec3f obj)
     //      {
     //           return new OVRPlugin.Vector3f{x = obj.x, y = obj.y, z = obj.z};
     //      }
     //      public static explicit operator Vector3(vec3f obj)
     //      {
     //           return new Vector3(obj.x, obj.y, obj.z);
     //      }
     // }
     
     [Serializable]
     public struct posef
	{
		public Quaternion Orientation;
		public Vector3 Position;

          public override string ToString()
          {
               return Position.ToString() + ","+ Orientation.ToString();
          }

          public static explicit operator OVRPlugin.Posef(posef obj)
          {
               return new OVRPlugin.Posef
               {
                    Orientation = new OVRPlugin.Quatf
                    {
                         x = obj.Orientation.x,
                         y = obj.Orientation.y,
                         z = obj.Orientation.z,
                         w = obj.Orientation.w
                         
                    }, 
                    Position = new OVRPlugin.Vector3f
                    {
                         x = obj.Position.x,
                         y = obj.Position.y,
                         z = obj.Position.z
                    }
               };
          }
          public static explicit operator posef(OVRPlugin.Posef obj)
          {
               return new posef
               {
                    Orientation = new Quaternion
                    {
                         x = obj.Orientation.x,
                         y = obj.Orientation.y,
                         z = obj.Orientation.z,
                         w = obj.Orientation.w
                    },
                    Position = new Vector3
                    {
                         x = obj.Position.x,
                         y = obj.Position.y,
                         z = obj.Position.z
                    }
               };
          }
	}
}
