using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataGather : MonoBehaviour
{
    OVRHand Hand;

    bool timeout = false;
    OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;

    [SerializeField]
    private GameObject _sphereObj;

    private GameObject[] tipSpheres;

    private int[] fingerTipsId = {
        (int)OVRSkeleton.BoneId.XRHand_ThumbTip,
        (int)OVRSkeleton.BoneId.XRHand_IndexTip,
        (int)OVRSkeleton.BoneId.XRHand_MiddleTip,
        (int)OVRSkeleton.BoneId.XRHand_RingTip,
        (int)OVRSkeleton.BoneId.XRHand_LittleTip,
    };
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject rightHandObj = GameObject.Find("OVRHandPrefab");
        Hand = rightHandObj.GetComponent<OVRHand>();
        _dataProvider = Hand as OVRSkeleton.IOVRSkeletonDataProvider;


        tipSpheres = new GameObject[5];

        for (int i = 0; i < tipSpheres.Length; i++) {
            Debug.Log(i);
            tipSpheres[i] = Instantiate(_sphereObj, Vector3.zero, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!timeout){
            StartCoroutine(showDataWait());
            //Debug.Log(" * * * * * * * * * * * * * * * * * * * * * * *");
            //Debug.Log("tip: " + (int)OVRSkeleton.BoneId.XRHand_IndexTip);
            //Debug.Log("is valid: " + Hand.IsDataValid);
            //Debug.Log(_dataProvider.GetSkeletonPoseData().BoneTranslations[(int)OVRSkeleton.BoneId.XRHand_IndexTip]);
            
            //if(!Hand.IsDataValid)return;

            // int id = 0;
            // foreach(GameObject obj in tipSpheres) {
            //     
            //     obj.transform.position = OVRExtensions.FromFlippedZVector3f(_dataProvider.GetSkeletonPoseData().BoneTranslations[fingerTipsId[id]]);
            //
            //     id++;
            // }
            
           
            
        }    
    }

    IEnumerator showDataWait(){
        timeout = true;
        yield return new WaitForSeconds(0.1f);
        timeout = false;
    }
}
