using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSaveLoad
{
  // TODO: Have the tilemaps stored in byte[] instead of strings for efficiency


  // converts data in the back/foreground tilemaps into strings
  public static string SaveGrid() {

    // Getting game objects and initializing List of Tilemaps
    GameObject[] allobjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    List<Tilemap> tilemaps = new();

    // Getting tilemaps
    foreach (GameObject i in allobjs)
    {
      Tilemap[] retval = i.GetComponentsInChildren<Tilemap>(true);
      tilemaps.AddRange(retval);
    }

    string gridData = ""; // strings containing tilemap data

    // converting background data into strings

    foreach (Tilemap map in tilemaps)
    {
      int x_min = map.cellBounds.min.x;
      int x_max = map.cellBounds.max.x;
      int y_min = map.cellBounds.min.y;
      int y_max = map.cellBounds.max.y;

      string name = map.name;
      Debug.Log($"SaveGrid: Processing {name} tilemap...");
      gridData += $"MAP {name}\n";

      for (int x = x_min; x < x_max; x++) {
        for (int y = y_min; y < y_max; y++) {
          Vector3Int loc = new Vector3Int(x, y, 0);
          Sprite sprite = map.GetSprite(loc);
          if (sprite != null) {
            string spriteName = sprite.name;
            Debug.Log("SaveGrid: spriteName: " + spriteName);
            gridData += spriteName + ", " + x + ", " + y + "\n";
          }
        }
      }

      gridData += "END\n";
    }
    
    return gridData;
  }

  // loads the world from the save file at the given path
  public static void LoadGrid(string gridData) {
    // Check parameter is good
    if (gridData.Length < 4 || gridData.IndexOf('\n') == -1)
    {
      Debug.LogError("LoadGrid: Bad tile load string!");
      return;
    }

    // Get the tilemaps, and create a parallel list of tilemap names
    GameObject[] allobjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    List<Tilemap> tilemaps = new();

    // Getting tilemaps
    foreach (GameObject i in allobjs)
    {
      Tilemap[] retval = i.GetComponentsInChildren<Tilemap>(true);
      tilemaps.AddRange(retval);
    }

    List<string> tilemapstrs = tilemaps.ConvertAll(x => x.name);

    // Load tile sprites
    Tile[] allTiles = Resources.LoadAll<Tile>("Tilemaps");

    // Load tilemap
    while (gridData.Length >= 4)
    {
      int endindex = gridData.IndexOf("END\n");
      if (gridData[..4] != "MAP " || endindex == -1) { break;}

      // Get the tilemap that we need to load
      int index = gridData.IndexOf('\n');
      string mapName = gridData[4..index];
      gridData = gridData[++index..];
      if (!tilemapstrs.Contains(mapName))
      {
        if (endindex+4 > gridData.Length) { break;}
        gridData = gridData[(endindex+4)..];
        continue;
      }
      Tilemap map = tilemaps[tilemapstrs.IndexOf(mapName)];

      for (int i = 0; i < gridData.Length; ++i)
      {
        if (i == endindex) 
        { 
          gridData = endindex+4 > gridData.Length ? "" : gridData[(i+4)..];
          break;
        }
        if (gridData[i] == ',')
        {
          string spritename = gridData[index..i];
          foreach (Tile t in allTiles)
          {
            if (t.sprite.name == spritename)
            {
              int endInt = gridData.IndexOf('\n',i);
              string mystr = gridData[(i+1)..endInt];
              i = endInt+1;
              string[] numbers = mystr.Split(',');
              if (numbers.Length != 2)
              {
                Debug.LogError("LoadGrid: Bad tile load string!");
              }

              int x = int.Parse(numbers[0]);
              int y = int.Parse(numbers[1]);

              Vector3Int loc = new(x,y,0);
              map.SetTile(loc,t);
              break;
            }
          }
          index = i;
        }
      }
    }

    /*
    // getting tiles
    Tile[] groundTiles = RenderDungeon.groundTiles;
    RuleTile wallTile = RenderDungeon.wallTile;
    */

  }
}
