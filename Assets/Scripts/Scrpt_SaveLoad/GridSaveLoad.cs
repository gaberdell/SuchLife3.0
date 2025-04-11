using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSaveLoad
{

  private const string BACKGROUND_NAME = "GroundTilemap"; // name of the background tilemap object
  private const string FOREGROUND_NAME = "PlaceableTileMap";

  // converts data in the back/foreground tilemaps into strings
  public static string SaveGrid() {
    
    // getting tilemaps
    
    GameObject background = GameObject.Find(BACKGROUND_NAME);
    if (background == null) {
      Debug.LogError("SaveGrid: Unable to find \"" + BACKGROUND_NAME + "\"!");
      return null;
    }

    GameObject foreground = GameObject.Find(FOREGROUND_NAME);
    if (foreground == null) {
      Debug.LogError("SaveGrid: Unable to find \"" + FOREGROUND_NAME + "\"!");
      return null;
    }

    // extracting Tilemap components
    
    Tilemap backTilemap = background.GetComponent<Tilemap>();
    if (backTilemap == null) {
      Debug.LogError("SaveGrid: Unable to extract " + BACKGROUND_NAME + "'s Tilemap component!");
      return null;
    }

    Tilemap foreTilemap = foreground.GetComponent<Tilemap>();
    if (foreTilemap == null) {
      Debug.LogError("SaveGrid: Unable to extract " + FOREGROUND_NAME + "'s Tilemap component!");
      return null;
    }

    string gridData = ""; // strings containing tilemap data

    // converting background data into strings

    int x_min = backTilemap.cellBounds.min.x;
    int x_max = backTilemap.cellBounds.max.x;
    int y_min = backTilemap.cellBounds.min.y;
    int y_max = backTilemap.cellBounds.max.y;

    Debug.Log("SaveGrid: Processing background tilemap...");

    gridData += "background\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {
        Vector3Int loc = new Vector3Int(x, y, 0);
        // Debug.Log("SaveGrid: loc: " + loc.ToString());

        Sprite sprite = backTilemap.GetSprite(loc);
        if (sprite != null) {
          // Debug.Log("SaveGrid: sprite: " + sprite.ToString());

          string spriteName = sprite.name;
          Debug.Log("SaveGrid: spriteName: " + spriteName);

          gridData += spriteName + ", " + x + ", " + y + "\n";
        }

      }
    }
    gridData += "END\n";

    // converting foreground data into strings

    x_min = foreTilemap.cellBounds.min.x;
    x_max = foreTilemap.cellBounds.max.x;
    y_min = foreTilemap.cellBounds.min.y;
    y_max = foreTilemap.cellBounds.max.y;

    Debug.Log("SaveGrid: Processing foreground tilemap...");

    gridData += "foreground\n";
    for (int x = x_min; x < x_max; x++) {
      for (int y = y_min; y < y_max; y++) {

        Vector3Int loc = new Vector3Int(x, y, 0);
        // Debug.Log("SaveGrid: loc: " + loc.ToString());

        Sprite sprite = foreTilemap.GetSprite(loc);
        if (sprite != null) {
          // Debug.Log("SaveGrid: sprite: " + sprite.ToString());

          string spriteName = sprite.name;
          Debug.Log("SaveGrid: spriteName: " + spriteName);

          gridData += spriteName + ", " + x + ", " + y + "\n";
        }

      }
    }
    gridData += "END\n";

    // returning
    return gridData;
  }

  // loads the world from the save file at the given path
  public static void LoadGrid(string namedSavePath) {
  
    // getting tilemaps

    GameObject background = GameObject.Find(BACKGROUND_NAME);
    if (background == null) {
      Debug.LogError("LoadGrid: Unable to find \"" + BACKGROUND_NAME + "\"!");
      return;
    }

    GameObject foreground = GameObject.Find(FOREGROUND_NAME);
    if (foreground == null) {
      Debug.LogError("LoadGrid: Unable to find \"" + FOREGROUND_NAME + "\"!");
      return;
    }

    // extracting Tilemap components

    Tilemap backTilemap = background.GetComponent<Tilemap>();
    if (backTilemap == null) {
      Debug.LogError("LoadGrid: Unable to extract " + BACKGROUND_NAME + "'s Tilemap component!");
      return;
    }

    Tilemap foreTilemap = foreground.GetComponent<Tilemap>();
    if (foreTilemap == null) {
      Debug.LogError("LoadGrid: Unable to extract " + FOREGROUND_NAME + "'s Tilemap component!");
      return;
    }

    // TODO: loading background
    RuleTile wallTile = RenderDungeon.wallTile;
    

    // TODO: loading foreground

  }
}
