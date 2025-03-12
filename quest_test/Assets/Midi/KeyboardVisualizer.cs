using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  PimDeWitte.UnityMainThreadDispatcher;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;


public class KeyboardVisualizer : MonoBehaviour
{
    public Material planeMat;

    private static List<int> whiteKeys = new List<int> { 0, 2, 4, 5, 7, 9, 11 };
    private static List<int> blackKeys = new List<int> { 1, 3, 6, 8, 10 };

    private static List<KeyVisualization> keyVisualizations;

    int leftKey;
    int rightKey;
    Vector3 leftCornerPosition;
    Vector3 rightCornerPosition;
    Vector3 forwardVector;

    public float blackKeyOffset = 2.5f;
    public float blackKeyHeight = 0.01f;

    public int numWhiteKeys = 17;

    float octaveWidth;

    private bool _hasConfiguration;

    // must be at the start of an octave, between b and c key
    Vector3 leftAnchor;
    int anchorKey;

    // startkey in the scale 0-11, c, c#, d, d#, e etc..
    int startKey;
    MIDIDevice _device;
    void Start()
    {
        _hasConfiguration = false;
        _device = GetComponent<MIDIDevice>();
        _device.OnNoteUpdate += NoteChanged;
        _configScript = GetComponent<ConfigurePhysicalKeyboard>();
        _configScript.OnActiveConfigChanged += configUpdate;
    }
    void NoteChanged(NoteEvent _){
        if(_hasConfiguration){
            UnityMainThreadDispatcher.Instance().Enqueue(UpdateKeyboard());
        }
    }
    public IEnumerator UpdateKeyboard(){
        for (int i = leftKey; i <= rightKey; i++) {
            keyVisualizations[i - leftKey].Update(_device.notesDown.Contains(i));
        }
        
        yield return null;
    }

    public class KeyVisualization{
        private Vector3 _keyPosition;
        private GameObject _plane;

        public KeyVisualization(int key, KeyboardVisualizer keyboardVisualizer){
            _keyPosition = keyboardVisualizer.getPositionFromKey(key);
            Debug.Log("spawning at pos: " + _keyPosition);
            Vector3 deltaVec = Vector3.Normalize(keyboardVisualizer.rightCornerPosition - keyboardVisualizer.leftCornerPosition);
            Vector3 keyVector = (keyboardVisualizer.octaveWidth * deltaVec) / 7.0f;
            _plane  = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.GetComponent<Renderer>().material = keyboardVisualizer.planeMat;
            float keyVisWidth = blackKeys.Contains(key%12) ?  keyVector.magnitude / 2.0f : keyVector.magnitude;
            _plane.transform.localScale = new Vector3(keyVisWidth, 1.0f, keyVector.magnitude*2) / 10.0f;
            _plane.transform.rotation = Quaternion.LookRotation(keyboardVisualizer.forwardVector);
            _plane.transform.position = _keyPosition + keyVector/2.0f + Vector3.Normalize(keyboardVisualizer.forwardVector) * keyVector.magnitude;
        }
        public void destroy(){
            Destroy(_plane);
        }
        public void Update(bool shouldRender){
            _plane.SetActive(shouldRender);
        }
    }

    Vector3 getPositionFromKey(int Key){
        int scaleKey = Key%12;
        Vector3 scalePos = getPositionFromScaleKey(scaleKey);
        Debug.Log("from scalePos: " + scalePos);
        Debug.Log("scale key is: " + scalePos);
        int octave = Key / 12;
        int anchorOctave = anchorKey / 12;
        Vector3 octaveVector = Vector3.Normalize(rightCornerPosition - leftCornerPosition) * octaveWidth;
        Vector3 octaveOffsetFromAnchor = (octave - anchorOctave) * octaveVector;
        return scalePos + octaveOffsetFromAnchor + leftAnchor;
    }

    Vector3 getPositionFromScaleKey(int scaleKey){
        Vector3 deltaVec = Vector3.Normalize(rightCornerPosition - leftCornerPosition);
        if(whiteKeys.Contains(scaleKey)){
            int i = whiteKeys.IndexOf(scaleKey);
            return (deltaVec * (octaveWidth/7.0f)) * i;
        }else if(blackKeys.Contains(scaleKey)){
            Vector3 oneKeyVector = (deltaVec * (octaveWidth/7.0f));
            Vector3 midPos;
            switch(scaleKey){
                case 1:
                    midPos = oneKeyVector * 1;
                    return midPos + (forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight - oneKeyVector * 0.75f;
                case 3:
                    midPos = oneKeyVector * 2;
                    return midPos + (forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight - oneKeyVector * 0.5f;
                case 6:
                    midPos = oneKeyVector * 4;
                    return midPos + (forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight - oneKeyVector * 0.5f;
                case 8:
                    midPos = oneKeyVector * 5;
                    return midPos + (forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight - oneKeyVector * 0.5f;
                case 10:
                    midPos = oneKeyVector * 6;
                    return midPos + (forwardVector * blackKeyOffset) + Vector3.up * blackKeyHeight - oneKeyVector * 0.5f;
                default:
                return Vector3.zero;
            }
        }
        return Vector3.zero;
    }

    
    private ConfigurePhysicalKeyboard _configScript;

    

    void configUpdate(ConfigurePhysicalKeyboard.Config config){
        Debug.Log(" ** UPDATING CONFIG ** ");
        
        leftKey = config.leftKey;
        rightKey = config.rightKey;
        leftCornerPosition = config.leftCornerPosition;
        rightCornerPosition = config.rightCornerPosition;
        forwardVector = config.forwardVector;

        startKey = leftKey % 12;
        
        octaveWidth = (((rightCornerPosition - leftCornerPosition) / numWhiteKeys) * 7.0f).magnitude;
        anchorKey = (leftKey/12) * 12;

        int i = whiteKeys.IndexOf(startKey);
        Vector3 deltaVec = Vector3.Normalize(rightCornerPosition - leftCornerPosition);
        Vector3 oneKeyVector = (deltaVec * (octaveWidth/7.0f));

        leftAnchor = leftCornerPosition - oneKeyVector * i;

        if(keyVisualizations != null){
            keyVisualizations.ForEach(keyVis => keyVis.destroy());
            keyVisualizations.Clear();
        }else{
            keyVisualizations = new List<KeyVisualization>();
        }
        

        int amountOfKeys = (rightKey - leftKey) + 1;
        for(int j = 0; j < amountOfKeys; j++){
            keyVisualizations.Add(
                new KeyVisualization(leftKey+j, this)
            );
        }
        _hasConfiguration = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
