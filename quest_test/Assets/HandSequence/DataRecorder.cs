using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

public class DataRecorder : MonoBehaviour
{

    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;

    OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;


    void logBones(){

        string[] logLines = new string[1];

        logLines[0] = getCurrentFrameLine();

        File.WriteAllLines("output.hseq", logLines);
    }


    string getCurrentFrameLine(){

        OVRSkeleton.SkeletonPoseData data = _dataProvider.GetSkeletonPoseData();

        string logLine = data.RootPose.Orientation.x.ToString() + 
            "," + data.RootPose.Orientation.y.ToString() + 
            "," + data.RootPose.Orientation.z.ToString() +
            "," + data.RootPose.Orientation.w.ToString() +
            "," + data.RootPose.Position.x.ToString() + 
            "," + data.RootPose.Position.y.ToString() +
            "," + data.RootPose.Position.z.ToString() +
            "," + data.RootScale.ToString();
            
        OVRPlugin.Quatf[] boneRotations = data.BoneRotations;
        for(int i = 0; i < boneRotations.Length; i++){
            logLine += "," + boneRotations[i].ToString();
        }

        logLine += "," + (data.IsDataValid ? "1" : "0"); 
        logLine += "," + (data.IsDataHighConfidence ? "1" : "0"); 

        OVRPlugin.Vector3f[] boneTranslations = data.BoneTranslations;
        for(int i = 0; i < boneTranslations.Length; i++){
            logLine += "," + boneTranslations[i].ToString();
        }

        logLine += "," + data.SkeletonChangedCount.ToString();

        return logLine.Replace(" ", "");;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) {
            logBones();
        }
    }
    void Start()
    {
        if (_dataProvider == null)
        {
            Debug.Log("Looking for data provider * * * * * * * * ** * * * ** * * " ); 
            var foundDataProvider = SearchSkeletonDataProvider();
            if (foundDataProvider != null)
            {
                Debug.Log("data provider found *** * * * * ** * * * * ** *");
                _dataProvider = foundDataProvider;
                if (_dataProvider is MonoBehaviour mb)
                {
                    Debug.Log($"Found IOVRSkeletonDataProvider reference in {mb.name} due to unassigned field.");
                }
            }else{
                Debug.Log("didnt find it* * * * * * * * ** * * * ** * * " ); 
            }
        }
    }

    internal  OVRSkeleton.IOVRSkeletonDataProvider SearchSkeletonDataProvider()
    {

        var oldProviders = gameObject.GetComponentsInParent<OVRSkeleton.IOVRSkeletonDataProvider>(true);
        foreach (var dataProvider in oldProviders)
        {
            Debug.Log("in loop* * * * * * * * ** * * * ** * * " ); 
 
            if (dataProvider.GetSkeletonType() == _skeletonType)
            {
                return dataProvider;
            }
        }

        return null;
    }
}
