To run: open quest_test unity project in unity in version 2022.3.46f1

* Enable either the Playback or RecordingGO gameobject and then run in unity. Both can't be enabled at the same time.
* Be sure that KeyboardConfiguration and MIDIDevice is also running.

The current version support one hand recording and playback. switch hand by enabling the OVRHandPrefab under Camera Rig 
building block. There you can also turn on and off visualizations over the live hand.

My mididevice is hard coded in MIDIDevice.cs, to use other devices change this string:
_inputDevice = InputDevice.GetByName("Nord Electro 5 MIDI");

In KeyboardConfiguration you map which keys are used for different functions. 
There you also specify the number of key you want to use. I used 17 between C3 and E4, which i know work. I haven't checked
for every bug here, so it might need some tweaking to get right.

Recordings are saved as .hseq files in te recording directory. Add them in the playback object in the inspector to play them.
