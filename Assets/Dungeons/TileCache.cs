using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// helper class for saving/loading dungeon tiles
[System.Serializable]
public class TileCache: Saveable
{

	public string name;
  public int x, y;

	public TileCache() {
		this.name = "";
		this.x = 0;
		this.y = 0;
	}

	public TileCache(string name, int x, int y)
	{
		this.name = name;
		this.x = x;
		this.y = y;
	}

	public string Save() {
			return JsonUtility.ToJson(this);
	}

	public void Load(string json) {
			JsonUtility.FromJsonOverwrite(json, this);
	}

	public void Load(List<string> jsons) { } // UNUSED
}
