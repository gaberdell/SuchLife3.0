using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;
using System.Buffers.Binary;

public class TilemapSaveLoad
{
  // Converts data in the back/foreground tilemaps into byte array
  public static byte[] SaveGrid() {

    // Getting game objects and initializing List of Tilemaps
    GameObject[] allobjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    List<Tilemap> tilemaps = new();

    // Getting tilemaps
    foreach (GameObject i in allobjs)
    {
      Tilemap[] maps = i.GetComponentsInChildren<Tilemap>(true);
      tilemaps.AddRange(maps);
    }

    // Our byte List with which we will store information (starts with 255 for verification)
    List<byte> retval = new() {255};

    // Start with dictionary of Tiles (Assumes there are <255 sprites whose names are <255 characters)
    Tile[] allTiles = Resources.LoadAll<Tile>("Tilemaps");
    if (allTiles.Length >= 255)
    {
      Debug.LogError("SaveGrid: Too many Tile sprites!");
      return null;
    }
    Dictionary<string,byte> tileDict = new();
    retval.Add(Convert.ToByte(allTiles.Length));

    for (int i = 0; i < allTiles.Length; ++i)
    {
      retval.Add(Convert.ToByte(allTiles[i].sprite.name.Length));
      retval.AddRange(Encoding.ASCII.GetBytes(allTiles[i].sprite.name));
      tileDict.Add(allTiles[i].sprite.name, Convert.ToByte(i));
    }
    retval.Add(0);

    // Saving Tilemaps into byte List
    foreach (Tilemap map in tilemaps)
    {
      int x_min = map.cellBounds.min.x;
      int x_max = map.cellBounds.max.x;
      int y_min = map.cellBounds.min.y;
      int y_max = map.cellBounds.max.y;

      string name = map.name;
      Debug.Log($"SaveGrid: Processing {name} tilemap...");

      // Add length of Tilemap name, then name itself
      retval.Add( (byte) (name.Length+2));
      retval.AddRange(Encoding.ASCII.GetBytes(name));

      // Add total length of Tilemap encoding
      byte[] myInt = new byte[4];
      BinaryPrimitives.WriteInt32LittleEndian(myInt,9*(x_max-x_min)*(y_max-y_min));
      retval.AddRange(myInt);

      // Add the Tiles in the Tilemap into the byte List
      for (int x = x_min; x < x_max; x++) {
        for (int y = y_min; y < y_max; y++) {
          Vector3Int loc = new(x, y, 0);
          Sprite sprite = map.GetSprite(loc);
          if (sprite != null) {
            string spriteName = sprite.name;
            Debug.Log("SaveGrid: spriteName: " + spriteName);
            retval.Add(tileDict[spriteName]); // 1 byte
            BinaryPrimitives.WriteInt32LittleEndian(myInt,x);
            retval.AddRange(myInt); // 4 bytes
            BinaryPrimitives.WriteInt32LittleEndian(myInt,y);
            retval.AddRange(myInt); // 4 bytes
          }
        }
      }
      retval.Add(255);
    }
    
    return retval.ToArray();
  }

  // Loads tilemap from byte array
  public static void LoadGrid(byte[] gridData) {
    // Check parameter is good
    if (gridData.Length == 0 || gridData[0] != 255)
    {
      Debug.LogError("LoadGrid: Bad tile load byte array!");
      return;
    }

    // Get the tilemaps, and create a parallel list of tilemap names
    GameObject[] allobjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    List<Tilemap> tilemaps = new();

    // Getting tilemaps and creating corresponding string Dictionary
    foreach (GameObject i in allobjs)
    {
      Tilemap[] retval = i.GetComponentsInChildren<Tilemap>(true);
      tilemaps.AddRange(retval);
    }
    Dictionary<string,Tilemap> tilemapDict = tilemaps.ToDictionary(x => x.name, x => x);

    // Load tile sprites and create corresponding string Dictionary
    Tile[] allTiles = Resources.LoadAll<Tile>("Tilemaps");
    Dictionary<string,Tile> tileDict = allTiles.ToDictionary(x => x.sprite.name, x => x);

    // Begin loading
    int index = 1;

    // Start by creating array of Tile sprite names
    string[] spriteNames = new string[gridData[index++]];
    for (int newIndex = 0; index < gridData.Length; ++newIndex)
    {
      if (newIndex == spriteNames.Length)
      {
        Debug.LogError("LoadGrid: Bad tile load byte array!");
        return;
      }
      int strlen = gridData[index++];
      if (strlen == 0) { break;}
      spriteNames[newIndex] = Encoding.ASCII.GetString(gridData,index,strlen);
      index += strlen;
    }

    if (spriteNames.Length >= 255)
    {
      Debug.LogError("LoadGrid: Bad tile load byte array!");
      return;
    }

    // Load tilemap
    for (; index < gridData.Length; )
    {
      int tilemapNameLen = gridData[index++];
      string tilemapName = Encoding.ASCII.GetString(gridData,index,tilemapNameLen);
      index += tilemapNameLen;
      int totalBytes = BinaryPrimitives.ReadInt32LittleEndian(gridData[index..]);
      if (!tilemapDict.ContainsKey(tilemapName))
      {
        index += totalBytes+4+1;
        continue;
      }

      index += 4;
      int totalTiles = totalBytes/9;
      Tilemap map = tilemapDict[tilemapName];

      for (int i = 0; i < totalTiles; ++i)
      {
        string spritestr = spriteNames[gridData[index++]];
        int x = BinaryPrimitives.ReadInt32LittleEndian(gridData[index..]);
        index += 4;
        int y = BinaryPrimitives.ReadInt32LittleEndian(gridData[index..]);
        index += 4;
        map.SetTile(new Vector3Int(x,y,0), tileDict[spritestr]);
      }

      if (gridData[index++] != 255)
      {
        Debug.LogError("LoadGrid: Bad tile load byte array!");
        return;
      }

    }
    
  }
}
