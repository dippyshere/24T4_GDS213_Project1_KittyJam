<br>
<div align=center>
    <div align="center">
        <img src="Assets/Textures/kitty jam logo.svg" alt="Kitty Jam" width="196">
    </div>
  <h6><em>Music and Cats collide in this vibrant rhythm adventure! Play your favourite songs in a variety of gameplay styles</em></h6>
</div>

___

## 🎵 About

Kitty Jam is a rhythm game featuring various gameplay styles for each song, leaderboards and accounts. This project is made with Unity 2022.3.16f1

## 🎮 Gameplay

- **Paw Percussion**: Tap the screen to the beat of the music
- **Feline Fretboard**: Play four lanes of notes in time with the music
- **Pouncing Parade**: Tap to the beat and move your wand
- **Bongo Bash**: Hit the bongos in time with the music

## 🎷🐈 Play

Play in WebGL, or download a build for your platform from the [Itch.io page](https://dippyshere.itch.io/kitty-jam)

## 🎶 Setup

1. Clone the repository
2. Open the project in Unity 2022.3.16f1 or later
3. Unless you have access to [this link](https://drive.google.com/file/d/1r6cxyxIjxzLtikmUN40AKqSxPkuj7eVd/view?usp=drive_link), you'll need some additional packages from the Unity Asset Store:

   [Beautify 3](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)

   [Gradient Shader Pack](https://assetstore.unity.com/packages/vfx/shaders/gradient-shader-pack-156673)

   [DoTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)

4. Move the imported packages to the `Assets/Imported` folder and create the missing `.asmdef` files (if manually imported)
5. If playing in editor, start from the `Bootstrap` scene, or from a `_xxBase` scene if you want to test a specific gameplay mode

## 🐾 Contributing

If contributing, please ensure your commits do not remove/modify any unintended files

### 🐱 Adding Songs

1. Use a DAW, such as [Reaper](https://www.reaper.fm/), to chart and export your song. Include a three measure count-in at the start, and align the first beat of the song with the third measure.
2. Follow this key for charting:

   **Paw Percussion**:
    - Place notes in C3 (60)

   **Feline Fretboard**:
    - Lane 1: E3 (64)
      - Lift notes: B2 (59)
    - Lane 2: D#3 (63)
      - Lift notes: A#2 (58)
    - Lane 3: D3 (62)
      - Lift notes: A2 (57)
    - Lane 4: C#3 (61)
      - Lift notes: G#2 (56)
    - To chart sustain notes, extend the duration of the note

   **Pouncing Parade**:
    - Place notes in F3 (65) - A#3 (70)

   **Bongo Bash**:
    - Outer Left: C#4 (73)
    - Inner Left: C4 (72)
    - Inner Right: B3 (71)
    - Outer Right: D4 (74)

3. Export your song in a compatible format, and place it in the `Assets/Audio/Songs` folder
4. Export your MIDI and place it in the `Assets/StreamingAssets` folder
5. Add the album art to the `Assets/Textures/AlbumArt` folder (ensure it is POT)
6. Create a new `SongData` asset in the `Assets/Songs` folder, and fill in the details
7. Add your song audio and album art to the **Songs** addressable group; and add a label for the song e.g. `songname` + `songasset` & `titledownload`. Then add the song data to the `SongData` addressable group; and add a label for the song e.g. `songname` + `songdata`
8. To test out your new song, set it as the default song in the **SongManager** script under the **Music** object in the **GameSystems** scene, then open a `_xxBase` scene to test that gameplay mode
