# TileCache.cs
### Cache Object

Implementing the ```Saveable``` interface, ```TileCache``` objects are created by ```RenderDungeon``` in its ```Save()``` and ```Load()``` methods, serving as a temporary data structure to save/load data for ```Tile```, including:

```spriteName```: the name of the sprite the tile is using

```x```, ```y```: the position of the tile

Creating a temporary object to store select data allows for ```JsonUtility``` to be used on these objects, rather than ```RenderDungeon``` as a whole, preventing unecessary data from being converted into JSON and returned as strings to be saved. As this class does not implement ```MonoBehavior```, the ```[System.Serializable]``` tag is added to allow ```JsonUtility``` to function properly.