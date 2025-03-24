using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFingerAssist : MonoBehaviour
{
    private SkeletonRenderer _sr;

    public HashSet<int> fingersDown;

    void Start()
    {
        fingersDown = new HashSet<int>();

        _sr = gameObject.GetComponent<SkeletonRenderer>();
        if(_sr == null){
            Debug.LogError("no skeleton renderer found in finger assist");
        }
    }

    public void AddFinger(int finger){
        Debug.Log("adding finger: " + finger);
        fingersDown.Add(finger);
    }

    public void RemoveFinger(int finger){
        Debug.Log("removing finger: " + finger);
        fingersDown.Remove(finger);
    }

    void Update()
    {
        
    }
}
