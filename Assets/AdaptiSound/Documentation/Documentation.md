# Documentation

## Files
![Imgur](https://i.imgur.com/hMaIGbr.png)
- The `AdaptiSound` folder contains the DEMO scenes, a mixer and the `AudioManager` folder, that contains all the scripts for this tool to work.
- If you want to try the DEMO, you will have to move the `Resources` folder to the root of Assets.

**Note:** *Adaptisound only needs the AudioManager folder to work, you can delete the DEMO and its resources if you want*


### `AudioManager Prefab`
![Imgur](https://i.imgur.com/cyypkwT.png)

![Imgur](https://i.imgur.com/qNAzFi7.png)

This prefab is in charge of managing all the playback methods for BGM and BGS.
The BGM, ABGM, and BGS objects are the containers for the audiosources that the tracks will play. They are separated into 3 categories:
- `BGM` (Background Music)
- `ABGM` (Adaptive Background Music)
- `BGS` (Background Sounds)

This prefab is a singleton, so you should only load it in your main scene

## AudioManager InspectorPanel
![Imgur](https://i.imgur.com/6CknjkF.png)

The `AudioManager` inspector will help you organize the audio files in your project.

### **Audio Directories**

You will need to assign a directory for each category (ABGM tracks include them in the BGM directory), `AudioManager` will search all subfolders for audio files with the extensions selected in **Extensions**.

*`Note1`: ABGM tracks must be created as prefabs. see `AdaptiNode` for more information.*

*`Note2`: The directories should be inside the Resources folder in Assets.*

### **Audio Buses**

Here you can assign an audio *BUS* for each category. This will help with later in-game audio volume management.

### **Debug**

- `Debugging`: You will be able to see the plugin's operation in the console.

## AudioManager Playback Methods

This tool is designed with the objective of implementing global background music and/or sounds. `AudioManager` will only play a single `BGM` or `ABGM` track, and in parallel one of `BGS`.

- The `current_playback` variable stores the only currently playing BGM or ABGM.
- The `current_bgs_playback` variable stores the only current playback of the BGS.

`Note:` *Remember to write the AdaptiSound namespace to use any of these methods*

![Imgur](https://i.imgur.com/B2fcZ75.png)

### `playMusic` 
<sub>Only for BGM/ABGM</sub>

![Imgur](https://i.imgur.com/HR90Zza.png)

This method will play from the beginning the audio with the name assigned in *track_name*. If there is already an audio being played, it will replace it, unless it is the same one, in which case, it will continue the current playback.

- `track_name:` type `String`, `AudioManager` will look for the preloaded sounds and play audio with this name.
- `volume:` type `Float`, set volume in dB of the track when played. `0.0f default`
- `fade_in:` type `Float`, set the fade time when the track is played. `0.5f default`
- `fade_out:` type `Float`, set the fade time when the current playback out. `0.5f default`
- `loop_index:` **only for AdaptiveTrack**, type `Int`, sets the index of the loop to be played after the intro. `0 default


### `stopMusic` 
<sub>Only for BGM/ABGM</sub>

![Imgur](https://i.imgur.com/Fp8PoN2.png)

This method stops the current playback.
- `can_fade:` type `Bool`, if true, apply fade_out on current playback track. `false default`
- `fade_out:` type `Float`, set the fade time when the current playback out. `1.5f default`


### `changeLoop` 
<sub>Only for ABGM</sub>

![Imgur](https://i.imgur.com/BixUb87.png)

- `track_name:` type `String`, name of the track on which the playing `Loop` will be changed.
- `loop_by_index:` type `Int`, loop index to play.
- `can_fade:` type `Bool`, if true, tracks will change with fades. `false default`
- `fade_out:` type `Float`, time of fade out. `0.5f default`
- `fade_in:` type `Float`, time of fade in. `0.5f default`


### `toOutro` 
<sub>Only for ABGM</sub>

![Imgur](https://i.imgur.com/NA94TKK.png)

- `track_name:` type `String`, name of the AdaptiveTrack in which to switch to the `Outro`
- `can_fade:` type `Bool`, if true, tracks will change with fades. `false default`
- `fade_out:` type `Float`, time of fade out. `0.5f default`
- `fade_in:` type `Float`, time of fade in. `0.5f default`


### `muteLayer` 
<sub>Only for ABGM</sub>

![Imgur](https://i.imgur.com/k76EyuC.png)

- `track_name:` type `String`, name of the track to mute or unmute for a specific layer.
- `layer:` type `Int`, target layer index.
- `mute_state:` type `Bool`, select to mute or unmute the asigned layer.
- `fade_time:` type `Float`, time of fade. `1.5f default`

### `muteAllLayer` 
<sub>Only for ABGM</sub>

![Imgur](https://i.imgur.com/5R9btkS.png)

- `track_name:` type `String`, name of the track on which to set all its layers to mute or unmute.
- `mute_state:` type `Bool`, select to mute or unmute all layers.
- `fade_time:` type `Float`, time of fade. `1.5f default`

### `muteLayerByName`
<sub>Only for ABGM</sub>

*This playback option not yet available.*

### `setSequence` 
<sub>Only for ABGM</sub>

![Imgur](https://i.imgur.com/LZathb1.png)

- `track_name:` type `String`, name of the track that will continue after the outro of the current track.

`Note:`*only available for tracks with outro.*

### `stopAll` 
<sub>For All</sub>

![Imgur](https://i.imgur.com/R4focq7.png)

This method stops all currently playing BGM/ABGM and BGS tracks, and removes them from the tree.
- `tracks_category:` type `String`, set a specific category you want to stop. `"ALL" default` (Use "BGM" or "BGS").
- `fade_time:` type `Float`, set the fade time of current playback. `0.5f default`


### `playSound`
<sub>Only for BGS</sub>

![Imgur](https://i.imgur.com/kIG8pXO.png)

This method will play from the beginning the audio with the name assigned in *track_name*. If there is already an audio being played, it will replace it, unless it is the same one, in which case, it will continue the current playback.

*Same as playMusic*

*Use `stopAll("BGS")` for stop sounds*


## AdaptiNode

To add adaptive music to our project we can use the `AdaptiNode`. The objective of these script is to create a single track that contains several audio tracks and a certain structure to be played.

First you need to go to the BGM directory and create a prefab in the folder you want. The name of this file will be the one you will use to call its playback.
After creating the file, you must add the `AdaptiNode` script to the prefab.

![Imgur](https://i.imgur.com/jFU12xt.png)

In this example I created a track called Theme1.

### Track Settings

The main function of `AdaptiNode` is to reproduce the following structure:

| Intro | Loops | Outro |
| ----- | ----- | ----- |
- `Intro:` It is a track that plays only once, and can only be stopped with a track change (`playMusic`), or with the `stopMusic` method.
- `Loops:` These tracks will play in a loop, but only one track will play at a time, to change from one loop to another you must call the `changeLoop` method.
- `Outro:` This track will play only once, and can only be interrupted by `stopMusic` method or a track change (`playMusic`). to go from the loops section to the outro you must call `toOutro` method.

Now to configure our adaptive track we open the prefab and we will find the following properties:

![Imgur](https://i.imgur.com/NnrO6lE.png)

- `intro_clip:` here you must add the audio file that is played as `Intro`. if you leave this field empty, the track will play from the `Loop` section.
- `outro_clip:` here you must add the audio file that is played as `Outro`. you can leave this field empty and just call `stopMusic` method.
- `measure_count:` if true, the console will print the track's bar count.
- `loops:` To add a loop you will need to follow some additional steps:

![Imgur](https://i.imgur.com/PckjoNV.png)

You can add multiple loops on the same track, but only **one** can be played at a time.
In each loop you can add **multiple layers**, and in this way create a loop with **parallel tracks**.

For each layer we have the following properties:

- `clip:` here you add the audio file that will be activated with this layer.
- `layer_name:` set a name for the layer, this will be used for the `muteLayerByName` method.
- `mute:` defines if this layer will be muted when playing the main track.

The objective of this structure is for all layers to play in parallel, and with the `muteLayer`, `muteAllLayer` or `muteLayerByName` methods we control their listening.

To continue, the following parameters allow the track's `beat count` and measure operation.
This feature will allow us to change loops, or change to the outro synchronously with the tempo of the music.

- `BPM:` beat per minute from track.
- `metric:` is the number of beats within a measure/bar.
- `total_beat_count:` is the total number of beats that the loop has. ***You must have this data to make the beats and bars counter work*** (An easy way to get it is to multiply the total number of bars x metric).
- `keys_loop_measure:` in this property you can assign keys to specific measures/bar, when the `changeLoop` method is called the track will be changed `only when the track enters one of these keys(measures/bar)`. If the above properties are not defined, then the track will instantly switch to another loop, or the outro.
- `keys_to_outro_measure:` in this property you can assign keys to specific measures/bar, when the `toOutro` method is called the track will be changed `only when the track enters one of these keys(measures/bar)`. If the above properties are not defined, then the track will instantly switch to another loop, or the outro.

## Other Methods

### get_bgm_track
![Imgur](https://i.imgur.com/g6TEJIT.png)

- `track_name:` type `String`, track name to get, return `Adapti_AudioSource`.

### get_abgm_track
![Imgur](https://i.imgur.com/rH6XGOC.png)

- `track_name:` type `String`, track name to get, return `AdaptiNode`.
### get_bgs_track
![Imgur](https://i.imgur.com/ex60TCR.png)

- `track_name:` type `String`, track name to get, return `Adapti_AudioSource`.

### removeTrack
![Imgur](https://i.imgur.com/SMWZE00.png)

- `track_name:` type `String`, name of the track which to remove from the scene.
Remove track instance and component.


