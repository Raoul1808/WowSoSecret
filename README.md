# Wow So Secret
Basically just toggleable secret mode in Spin Rhythm XD.

# About secret texts
The format used for custom secret texts is JSON. **Make sure you follow correctly JSON conventions when adding new texts.**

For example:
```json
{
  "Editor": [
    "Secret Mode Enabled!",   <-- INVALID: there is no value after this one, the comma shouldn't be there.
  ],
  "Playing": [
    "Secret Mode Enabled!",
    "My Second Secret Text"   <-- INVALID: there is a value afterwards, the comma is NECESSARY.
    "Hello, World!"
  ],
  "Results": [
    "Secret Mode Enabled!",
    "Viewing secret results"
  ]
}
```

# Installing the mod
1. Make sure your BepInEx installation isn't borked
2. Grab the latest dll in the releases tab
3. Put it in your plugins folder
4. Run
5. Remember to head over to the config and adjust settings as you like

# Building
1. Replace all the /ssd/SteamLibrary/rest-of-path references with the actual paths to your game.
2. Build

# License
This project is released under the MIT License.
