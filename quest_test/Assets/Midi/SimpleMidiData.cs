using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

public class SimpleMidiData : MonoBehaviour, MIDIDevice.MidiDataProvider
{
    private MIDIDevice _device;
    private List<HandSequence.SerializableNoteEvent> _eventBuffer;

    public List<HandSequence.SerializableNoteEvent> GetMidiData(){
        // returning and reseting buffer
        var oldBuffer = _eventBuffer;
        _eventBuffer = new List<HandSequence.SerializableNoteEvent>();
        return oldBuffer;
    }

    void Start()
    {
        _device = GetComponent<MIDIDevice>();
        _device.OnNoteUpdate += NoteChanged;
        _eventBuffer = new List<HandSequence.SerializableNoteEvent>();
        
        //keyboard.EventSent += OnEventSent;

    }
    void NoteChanged(NoteEvent e){
        _eventBuffer.Add(new HandSequence.SerializableNoteEvent(e));
    }

    // Update is called once per frame
    void Update(){
    
    }
}
