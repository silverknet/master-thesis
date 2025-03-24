using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Utils to get hand-keyboard related data
public class HandUtil : MonoBehaviour
{

    [SerializeField]
    private OVRSkeleton.SkeletonType _skeletonType;

    private ConfigurePhysicalKeyboard _configScript;
    private ConfigurePhysicalKeyboard.Config _config;
    
    private HandSequence.SkeletonHandSequenceProvider _dataProvider;

    private Matrix4x4 _m;

    public float blackKeyOffset = 2.5f;
    public float blackKeyHeight = 0.01f;

    private static List<int> whiteKeys = new List<int> { 0, 2, 4, 5, 7, 9, 11 };
    private static List<int> blackKeys = new List<int> { 1, 3, 6, 8, 10 };

    


    void Start()
    {
        
    }
    void Awake(){

        _dataProvider = SearchSkeletonDataProvider();
        if(_dataProvider == null){ Debug.LogError("No data provider for util");
            return;
        }

        _configScript = SearchConfig();
        if(_configScript == null){ Debug.LogError("No config found for util");
            return;
        }

        _configScript.OnActiveConfigChanged += configUpdate;
    }
    private void configUpdate(ConfigurePhysicalKeyboard.Config _){
        Debug.Log("Config update inside Util");
        _config = _configScript.activeConfig;
        _m = _configScript.getInverseSpaceMatrix();
    }

    //public int GetFingerFromKey(int key){
    //    return 50;
    //}

    public int GetFingerFromKey(int key){
        Vector3[] BoneTranslations = _dataProvider.GetHandFrameData().BoneTranslations;
        Debug.Log(BoneTranslations[(int)OVRHandData.ovrHandEnum.ThumbTip]);

        Vector3[] fingertipPositions = new Vector3[5]
        {
            BoneTranslations[(int)OVRHandData.ovrHandEnum.ThumbTip],
            BoneTranslations[(int)OVRHandData.ovrHandEnum.IndexTip],
            BoneTranslations[(int)OVRHandData.ovrHandEnum.MiddleTip],
            BoneTranslations[(int)OVRHandData.ovrHandEnum.RingTip],
            BoneTranslations[(int)OVRHandData.ovrHandEnum.LittleTip]
        };

        int anchorKey = _config.anchorKey;
        float[] xPos = new float[5];

        for (int i = 0; i < fingertipPositions.Length; i++) 
        {
            xPos[i] = _m.MultiplyPoint(fingertipPositions[i]).x;
        }

        Vector3 keyPos = getMidPositionFromKey(key);

       
        keyPos = _m.MultiplyPoint(keyPos);
        // when we transform into keyboard space, the keyboard keys goes along the X axis, the forward vector will go into the Z axis, and up is Y.
        // We dont care about Z and Y so we can ignore them. And only check which finger pos is closest to X.
        Debug.Log("keypos: " + keyPos);
        Debug.Log("xpos: " + xPos);
        return ClosestFinger(keyPos.x, xPos);
    }

    public static int ClosestFinger(float target, float[] numbers)
    {
        return Array.IndexOf(numbers, numbers.OrderBy(n => Math.Abs(n - target)).First());
    }

    Vector3 getMidPositionFromKey(int Key){
        int scaleKey = Key%12;
        Vector3 scalePos = getMidPositionFromScaleKey(scaleKey); 
        int octave = Key / 12;
        int anchorOctave = _config.anchorKey / 12;
        Vector3 octaveVector = Vector3.Normalize(_config.rightCornerPosition - _config.leftCornerPosition) * _config.octaveWidth;
        Vector3 octaveOffsetFromAnchor = (octave - anchorOctave) * octaveVector;
        return scalePos + octaveOffsetFromAnchor + _config.anchor;
    }

    private Vector3 getMidPositionFromScaleKey(int scaleKey){
        if(whiteKeys.Contains(scaleKey)){
            int i = whiteKeys.IndexOf(scaleKey);
            return (_config.deltaVec * (_config.octaveWidth/7.0f)) * i + _config.oneKeyVector/2;
        }else if(blackKeys.Contains(scaleKey)){
            Vector3 oneKeyVector = (_config.deltaVec * (_config.octaveWidth/7.0f));
            Vector3 midPos;
            switch(scaleKey){
                case 1:
                    midPos = _config.oneKeyVector * 1;
                    return midPos + (_config.forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight;
                case 3:
                    midPos = _config.oneKeyVector * 2;
                    return midPos + (_config.forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight;
                case 6:
                    midPos = _config.oneKeyVector * 4;
                    return midPos + (_config.forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight;
                case 8:
                    midPos = _config.oneKeyVector * 5;
                    return midPos + (_config.forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight;
                case 10:
                    midPos = _config.oneKeyVector * 6;
                    return midPos + (_config.forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight;
                default:
                return Vector3.zero;
            }
        }
        return Vector3.zero;
    }
    

    internal ConfigurePhysicalKeyboard SearchConfig(){
        if(_configScript == null) {
            var configGO = GameObject.Find("KeyboardConfiguration");
            var _configScript = configGO ? configGO.GetComponent<ConfigurePhysicalKeyboard>() : null;
            if(_configScript != null){
                return _configScript;
            }else{Debug.LogError("No config found");}
        }
        return null;
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

}
