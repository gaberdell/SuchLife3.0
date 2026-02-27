using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEffectsDict : MonoBehaviour 
{
    //instantiate and return dictionary of tile effects; putting it in its own script for organization sake
    Dictionary<TileBase, StatusEffect> dict = new Dictionary<TileBase, StatusEffect>();
    [SerializeField] TileBase spikeUpTile;
    [SerializeField] TileBase pitTile;
    public Dictionary<TileBase, StatusEffect> createDict()
    {
        BaseEffect[] hurtfxs = new BaseEffect[1];
        hurtfxs[0] = new HurtEffect(1, 0);
        StatusEffect hurtEffect = new StatusEffect(0, hurtfxs, null, null);
        dict.Add(spikeUpTile, hurtEffect);
        return dict;
    }

    public Dictionary<TileBase, StatusEffect> getDict()
    {
        if(dict.Count <= 0) return createDict();
        return dict;
    }
}
