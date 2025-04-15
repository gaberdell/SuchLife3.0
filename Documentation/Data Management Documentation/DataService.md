# DataService.cs
### MonoBehavior Script

This script acts as a library for saving, loading, and fetching game states. Attached to a DataMgnt ```GameObject```, this allows for other ```GameObject```s to easily find and call methods within this script. This script includes the following:

```struct SaveInfo```: a simple struct containing various details of a save file:
 - ```string path```: the path of the save file
 - ```string name```: the name of the save file
 - ```DataTime lastModified```: a ```DateTime``` object representing the last time a game state was saved into the save file

```public List<SaveInfo> Fetch()```: Returns a list of ```SaveInfo``` structs, representing every save file. This list is sorted by ```lastModified```.

```public bool Save(string path)```: Saves the current game state into the given path, returns success through a boolean.

```public bool Load(string path)```: Loads the game state at the given path, returns success through a boolean.

```public bool Delete(string path)```: Deletes the save file at the given path, returns success through a boolean.