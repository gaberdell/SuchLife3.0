using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSaveLoad
{

  private const string BACKGROUND_NAME = "GroundTilemap"; // name of the background tilemap object
  private const string FOREGROUND_NAME = "PlaceableTileMap";

  // converts data in the back/foreground tilemaps into JSON strings
  public static string SaveGrid() {
    
    // getting tilemaps
    
    GameObject background = GameObject.Find(BACKGROUND_NAME);
    if (background == null) {
      Debug.LogError("GridSaveLoad: Unable to find \"" + BACKGROUND_NAME + "\"!");
      return null;
    }

    GameObject foreground = GameObject.Find(FOREGROUND_NAME);
    if (foreground == null) {
      Debug.LogError("GridSaveLoad: Unable to find \"" + FOREGROUND_NAME + "\"!");
      return null;
    }

    // extracting Tilemap components
    
    Tilemap backTilemap = background.GetComponent<Tilemap>();
    if (backTilemap == null) {
      Debug.LogError("GridSaveLoad: Unable to extract " + BACKGROUND_NAME + "'s Tilemap component!");
      return null;
    }

    Tilemap foreTilemap = foreground.GetComponent<Tilemap>();
    if (foreTilemap == null) {
      Debug.LogError("GridSaveLoad: Unable to extract " + FOREGROUND_NAME + "'s Tilemap component!");
      return null;
    }

    string gridJSON = ""; // string containing JSON-formatted strings of tilemap data

    // converting background into JSON strings

    int x_min = backTilemap.cellBounds.min.x;
    int x_max = backTilemap.cellBounds.max.x;
    int y_min = backTilemap.cellBounds.min.y;
    int y_max = backTilemap.cellBounds.max.y;

    gridJSON += "background\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {
        Tile tile = (Tile)backTilemap.GetTile(new Vector3Int(x, y, 0));

        if (tile != null) {
          TileCache tileCache = new TileCache(tile.sprite.name, x, y); // saving tile data in a temporary object

          string spriteName = tileCache.spriteName;
          gridJSON += tileCache.Save() + "\n";
        }
      }
    }
    gridJSON += "END\n";

    x_min = foreTilemap.cellBounds.min.x;
    x_max = foreTilemap.cellBounds.max.x;
    y_min = foreTilemap.cellBounds.min.y;
    y_max = foreTilemap.cellBounds.max.y;

    gridJSON += "foreground\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {
        Tile tile = (Tile)foreTilemap.GetTile(new Vector3Int(x, y, 0));

        if (tile != null) {
          TileCache tileCache = new TileCache(tile.sprite.name, x, y); // saving tile data in a temporary object

          string spriteName = tileCache.spriteName;
          gridJSON += tileCache.Save() + "\n";
        }
      }
    }
    gridJSON += "END\n";

    // returning
    return gridJSON;
  }
}
