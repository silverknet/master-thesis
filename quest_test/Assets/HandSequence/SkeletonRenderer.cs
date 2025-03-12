using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Meta.WitAi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

/// <summary>
/// Skeleton Renderer, made for rendering handframes. Can be used with OVRSkeleton, but it has to be converted to handframes first.
/// </summary>
public class SkeletonRenderer : MonoBehaviour
{
    private HandSequence.SkeletonHandSequenceProvider _dataProvider;
    
    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;
    
    private List<BoneVisualization> _boneVisualizations;

    private bool _isRendering = false;
    
    private static readonly Quaternion _capsuleRotationOffset = Quaternion.Euler(90, 0, 0);

    public static GameObject _handGO;

    
    
    void Start()
    {
        _dataProvider = SearchSkeletonDataProvider();
        if(_dataProvider == null){ Debug.Log("No data provider for renderer");
            return;
        }
    }
    private class BoneVisualization
    {
        private Vector3 _bonePosition;
        private BoneVisualization _parent;
        private Vector3 _delta;
        public OVRHandData.ovrHandEnum ID;
        
        public Vector3 BonePosition { 
            get => _bonePosition;
            set { _bonePosition = value; }
        }
        private Quaternion _boneRotation;
        public Quaternion BoneRotation { 
            get => _boneRotation;
            set { _boneRotation = value; }
        }
        
        private GameObject jointGO;
        private GameObject boneGO;

        public bool ShouldRender { get; set; }
        public BoneVisualization(int index, BoneVisualization parent, Vector3 refPosition, BoneVisualization wrist)
        {
            ID = OVRHandData.GetJointIDfromIndex(index);
            _parent = parent;
            _bonePosition = refPosition;
            
            jointGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jointGO.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            jointGO.transform.SetParent(_handGO.transform, true);
            
            //if (parent == null) return; // dont create bone if joint has no parent.
            //Debug.Log("-- INIT BONE --");
            //Debug.Log("type: " + OVRHandData.GetJointIndex(ID) + ". " + ID + " position: " + BonePosition);
            
            //Debug.Log("parent type: " + _parent.ID.ToIndex() + ". " + _parent.ID + " position: " + _parent.BonePosition);
            //Debug.Log("delta: " + (_bonePosition - _parent.BonePosition));
            //Debug.Log("deltamag: " + (_bonePosition - _parent.BonePosition).magnitude);
            //Debug.Log(" -- end -- ");

            boneGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            boneGO.name = ("bone-" + ID.ToString());

            boneGO.transform.SetParent(_handGO.transform, true);

            /*if(OVRHandData.hasParent.Contains(index)){
                boneGO.transform.SetParent(parent.boneGO.transform, true);
            }
            if(OVRHandData.hasParentWrist.Contains(index)){
                boneGO.transform.SetParent(wrist.boneGO.transform, true);
            }*/
            
            float bone_length = 1.0f;
            if (parent != null) {
                _delta = _bonePosition - _parent.BonePosition;
                bone_length = _delta.magnitude;
            }
            
            boneGO.transform.localScale = new Vector3(0.002f, bone_length/2, 0.002f);
        }

        public void Update()
        {


            if (_parent != null)
            {
                _delta = _bonePosition - _parent.BonePosition;
                boneGO.transform.position = BonePosition - _delta/2;
                boneGO.transform.localRotation = BoneRotation * _capsuleRotationOffset;
                boneGO.SetActive(ShouldRender);
            }
            
            jointGO.transform.localPosition = BonePosition;
            
            jointGO.SetActive(ShouldRender);
            if (!ShouldRender) return;
        }
        public void DestroyVis()
        {
            Destroy(jointGO);
            if (_parent != null)
            {
                Destroy(boneGO);
            }
        }
    }

    private void Initialize()
    {
        _isRendering = true;
        _boneVisualizations = new List<BoneVisualization>();
        HandSequence.HandFrame data = _dataProvider.GetHandFrameData();
        _handGO = new GameObject();


        for (int i = 0; i < data.BoneTranslations.Length; i++)
        {
            //Debug.Log("index: " + i);
            //Debug.Log("parent: " + OVRHandData.jointsCustom[i].Parent + "parent index: " + (int)OVRHandData.jointsCustom[i].Parent);
            //Debug.Log("parent index: " + OVRHandData.GetJointIndex(OVRHandData.jointsCustom[i].Parent));
            
            OVRHandData.ovrHandEnum parent = OVRHandData.jointsCustom[i].Parent;

            //Debug.Log("inavlid parent: " + (parent == OVRHandData.ovrHandEnum.Invalid));
            
            //int bone_i = i==0?1:(i==1?0:i);
            
            _boneVisualizations.Add(new BoneVisualization(
                i, 
                parent!=OVRHandData.ovrHandEnum.Invalid?_boneVisualizations[OVRHandData.GetJointIndex(parent)]:null, 
                OVRExtensions.FromFlippedZVector3f(new OVRPlugin.Vector3f{x = data.BoneTranslations[i].x, y = data.BoneTranslations[i].y, z = data.BoneTranslations[i].z}),
                _boneVisualizations.Count>1?_boneVisualizations[1]:null
                ));
        }
        
    }

    private void clearVisualization()
    {
        foreach (var boneVis in _boneVisualizations)
        {
            boneVis.DestroyVis();
        }
        _boneVisualizations.Clear();
    }
    internal HandSequence.SkeletonHandSequenceProvider SearchSkeletonDataProvider()
    {
        var oldProviders = gameObject.GetComponentsInParent<HandSequence.SkeletonHandSequenceProvider>();
        foreach (var dataProvider in oldProviders)
        {
            if (dataProvider.GetSkeletonType() == _skeletonType)
            {
                return dataProvider;
            }
        }

        return null;
    }
    
    void Update()
    {
        if (_dataProvider == null) return;
        HandSequence.HandFrame data = _dataProvider.GetHandFrameData();
        
        if (data == null)
        {
            if (_isRendering)
            {
                _isRendering = false;
                clearVisualization();
            }
            return;
        }

        if (!_isRendering)
        {
            Initialize();
        }

        OVRPlugin.Vector3f handPosition = new OVRPlugin.Vector3f{x = data.RootPose.Position.x, y = data.RootPose.Position.y, z = data.RootPose.Position.z};
        OVRPlugin.Quatf handRotation = new OVRPlugin.Quatf{x = data.RootPose.Orientation.x, y = data.RootPose.Orientation.y, z = data.RootPose.Orientation.z, w = data.RootPose.Orientation.w};
        //Quaternion rot = handRotation.FromFlippedZQuatf();
        //Vector3 pos = handPosition.FromFlippedZVector3f();

        //_handGO.transform.localRotation = rot;
        //_handGO.transform.localPosition = pos;
        //_handGO.transform.localScale = Vector3.one * data.RootScale;

        

        for (int i = 0; i < _boneVisualizations.Count; i++)
        {
            //int bone_i = i==0?1:(i==1?0:i);
            _boneVisualizations[i].BonePosition = (new OVRPlugin.Vector3f{x = data.BoneTranslations[i].x, y = data.BoneTranslations[i].y, z = data.BoneTranslations[i].z}).FromFlippedZVector3f();
            _boneVisualizations[i].BoneRotation = (new OVRPlugin.Quatf{x = data.BoneRotations[i].x, y = data.BoneRotations[i].y, z = data.BoneRotations[i].z,w = data.BoneRotations[i].w}).FromFlippedZQuatf();
            _boneVisualizations[i].ShouldRender = data.IsDataValid;
            _boneVisualizations[i].Update();
        }
    }
    
}
