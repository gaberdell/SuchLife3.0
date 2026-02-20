using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileEffectsDict 
{
    //instantiate and return dictionary of tile effects; putting it in its own script for organization sake
    static Dictionary<TileBase, StatusEffect> dict = new Dictionary<TileBase, StatusEffect>();
    public static Dictionary<TileBase, StatusEffect> createDict()
    {
        return dict;
    }
}
