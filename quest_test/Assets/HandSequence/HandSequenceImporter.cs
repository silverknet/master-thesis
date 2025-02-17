#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.Collections.Generic;
using UnityEditor;


/// <summary>
/// Importer for a OpenXR animated hand sequence, recorded from 
/// 
/// file structure, one line correponds to one frame, all of the below will be on one line
/// rootpose.orientation.x, rootpose.orientation.y, rootpose.orientation.z, rootpose.orientation.w, 
/// rootpose.Position.x, rootpose.Position.y, rootpose.Position.z, 
/// rootScale(float), 
/// [bonerotations(quatf)*26], 
/// isdatavalid(bool), 
/// isdatahighconfidence(bool),
/// [bonetranslatations(quatf)*26],
/// skeletonchangedcount(int)
///
/// </summary>
[ScriptedImporter(1, new string[] { "hseq" })]
public class HandSequenceImporter : ScriptedImporter {
    

    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {
        using (var file = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(file)) {
            float fileLength = file.Length;

            HandSequence handSequence = ScriptableObject.CreateInstance<HandSequence>();
            handSequence.frames = new List<HandSequence.HandFrame>();
            
            string info = $"Reading particle set file {Path.GetFileName(ctx.assetPath)}";
            EditorUtility.DisplayProgressBar("Importing Hand Sequence", info, 0);

            int currentFrame = 0;
            while (true)
            {
                string line = reader.ReadLine(); // Read a new line
                if (line == null) break; // stop at end of file
                

                var element = line.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();

                handSequence.frames.Add(new HandSequence.HandFrame());
                
                OVRPlugin.Posef tempRootPose = new OVRPlugin.Posef();

                tempRootPose.Orientation.x = element[0];
                tempRootPose.Orientation.y = element[1];
                tempRootPose.Orientation.z = element[2];
                tempRootPose.Orientation.w = element[3];
                tempRootPose.Position.x = element[4];
                tempRootPose.Position.y = element[5];
                tempRootPose.Position.z = element[6];
                handSequence.frames[currentFrame].frameData.RootPose = tempRootPose;

                handSequence.frames[currentFrame].frameData.RootScale = element[7];
                
                int bones_amount = 26;
                int start_index = 8;
                handSequence.frames[currentFrame].frameData.BoneRotations = new OVRPlugin.Quatf[bones_amount]; 
                for(int i = 0; i < bones_amount; i++){
                    handSequence.frames[currentFrame].frameData.BoneRotations[i].x = element[start_index+i*4];
                    handSequence.frames[currentFrame].frameData.BoneRotations[i].y = element[start_index+i*4+1];
                    handSequence.frames[currentFrame].frameData.BoneRotations[i].z = element[start_index+i*4+2];
                    handSequence.frames[currentFrame].frameData.BoneRotations[i].w = element[start_index+i*4+3];
                }
                int next_element = start_index + bones_amount*4;

                handSequence.frames[currentFrame].frameData.IsDataValid = element[next_element] == 1.0f;
                handSequence.frames[currentFrame].frameData.IsDataHighConfidence = element[next_element+1] == 1.0f;

                start_index = next_element+1;
                handSequence.frames[currentFrame].frameData.BoneTranslations = new OVRPlugin.Vector3f[bones_amount]; 
                for(int i = 0; i < bones_amount; i++){
                    handSequence.frames[currentFrame].frameData.BoneTranslations[i].x = element[start_index+i*3];
                    handSequence.frames[currentFrame].frameData.BoneTranslations[i].y = element[start_index+i*3+1];
                    handSequence.frames[currentFrame].frameData.BoneTranslations[i].z = element[start_index+i*3+2];
                }
                next_element = start_index + bones_amount*3;

                handSequence.frames[currentFrame].frameData.SkeletonChangedCount = (int)element[next_element];

                
                ++currentFrame;
                if (currentFrame % 100 == 0) {
                    EditorUtility.DisplayProgressBar("Particle Set Importer", info, file.Position / fileLength);
                }
                
            }

            handSequence.length = currentFrame;
            
            ctx.AddObjectToAsset("Hand.Sequence", handSequence);
            ctx.SetMainObject(handSequence);
            
            EditorUtility.ClearProgressBar();

        }
    }

}
#endif
