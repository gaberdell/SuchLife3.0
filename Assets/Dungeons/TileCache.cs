using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// helper class for saving/loading dungeon tiles
[System.Serializable]
public class TileCache: Saveable
{

	public string spriteName; // name of the Sprite used in the Tile
  public int x, y;

	public TileCache() {
		this.spriteName = "";
		this.x = 0;
		this.y = 0;
	}

	public TileCache(string name, int x, int y)
	{
		this.spriteName = name;
		this.x = x;
		this.y = y;
	}

	public string Save() {
		return JsonUtility.ToJson(this);
	}

	public void Load(string json) {

		Debug.Log("Loading " + json + " ...");

		JsonUtility.FromJsonOverwrite(json, this);
	}

}
