using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class OVRHandConverter : MonoBehaviour, HandSequence.SkeletonHandSequenceProvider
{
    [SerializeField] private OVRSkeleton.SkeletonType _skeletonType;
    private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;
    
    public OVRSkeleton.SkeletonType GetSkeletonType()
    {
        return _skeletonType;
    }

    public HandSequence.HandFrame GetHandFrameData()
    {
        return (HandSequence.HandFrame)_dataProvider.GetSkeletonPoseData();
    }
    
    public bool IsInitialized()
    {
        return true;
    }

    internal OVRSkeleton.IOVRSkeletonDataProvider SearchSkeletonDataProvider()
    {
        GameObject obj = GameObject.Find("OVRHandPrefab");
            
        var providers = obj.GetComponentsInParent<OVRSkeleton.IOVRSkeletonDataProvider>(true);
        foreach (var dataProvider in providers)
        {
            if (dataProvider.GetSkeletonType() == _skeletonType)
            {
                return dataProvider;
            }
        }
        return null;
    }

    public void OnValidate()
    {
        
        if (!TryGetComponent<OVRSkeleton>(out OVRSkeleton skeleton))
        {
            Debug.LogWarning("No OVRSkeleton present to convert");
            return;
        }
        
        if (skeleton.GetSkeletonType() != _skeletonType)
        {
            MethodInfo setSkeletonTypeMethod = typeof(OVRSkeleton).GetMethod("SetSkeletonType",
                BindingFlags.Instance | BindingFlags.NonPublic); // Access protected method

            if (setSkeletonTypeMethod != null)
            {
                setSkeletonTypeMethod.Invoke(skeleton, new object[] { _skeletonType });
            }
            else
            {
                Debug.LogError("SetSkeletonType() method not found");
            }
        }
    }

    private void Start()
    {
        if (_dataProvider == null)
        {
            var foundDataProvider = SearchSkeletonDataProvider();
            if (foundDataProvider != null)
            {
                _dataProvider = foundDataProvider;
                if (_dataProvider is MonoBehaviour mb)
                {
                    Debug.Log($"Found IOVRSkeletonDataProvider reference in {mb.name} due to unassigned field.");
                }
            }else{
                Debug.LogWarning("didn't find a data provider for recording" ); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
