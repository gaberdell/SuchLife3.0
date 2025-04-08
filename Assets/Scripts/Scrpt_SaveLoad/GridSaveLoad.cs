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

    Debug.Log("GridSaveLoad: Processing background tilemap...");

    gridJSON += "background\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {
        Vector3Int loc = new Vector3Int(x, y, 0);
        // Debug.Log("GridSaveLoad: loc: " + loc.ToString());

        Sprite sprite = backTilemap.GetSprite(loc);
        if (sprite != null) {
          // Debug.Log("GridSaveLoad: sprite: " + sprite.ToString());

          string spriteName = sprite.name;
          Debug.Log("GridSaveLoad: spriteName: " + spriteName);

          TileCache tileCache = new TileCache(spriteName, x, y); // TOFIX: optimize
          gridJSON += tileCache.Save() + "\n";
        }

      }
    }
    gridJSON += "END\n";

    // converting foreground into JSON strings

    x_min = foreTilemap.cellBounds.min.x;
    x_max = foreTilemap.cellBounds.max.x;
    y_min = foreTilemap.cellBounds.min.y;
    y_max = foreTilemap.cellBounds.max.y;

    Debug.Log("GridSaveLoad: Processing foreground tilemap...");

    gridJSON += "foreground\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {

        Vector3Int loc = new Vector3Int(x, y, 0);
        // Debug.Log("GridSaveLoad: loc: " + loc.ToString());

        Sprite sprite = foreTilemap.GetSprite(loc);
        if (sprite != null) {
          // Debug.Log("GridSaveLoad: sprite: " + sprite.ToString());

          string spriteName = sprite.name;
          Debug.Log("GridSaveLoad: spriteName: " + spriteName);

          TileCache tileCache = new TileCache(spriteName, x, y); // TOFIX: optimize
          gridJSON += tileCache.Save() + "\n";
        }

      }
    }
    gridJSON += "END\n";

    // returning
    return gridJSON;
  }
}
