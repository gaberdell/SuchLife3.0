# Saveable
### Interface

All scripts that have data that needs to be saved should include this interface. Consists of two functions:

```public string Save()``` - returns a JSON-formatted string

```public void Load(string json)``` - requires a JSON-formatted string

In these functions should be code that converts JSON strings into data (or vice-versa), either through ```JsonUtility``` or some other valid method. The use of temporary cache objects is recommended (see ```TileCache```). ```DataService``` calls the ```Save()``` and ```Load()``` methods from the necessary scripts using this interface to call for JSON data to save, and to deliver JSON string(s) to load.