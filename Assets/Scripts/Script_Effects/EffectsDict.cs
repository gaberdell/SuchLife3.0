using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EffectsDict : MonoBehaviour 
{
    //instantiate and return dictionary of tile effects; putting it in its own script for organization sake
    Dictionary<TileBase, StatusEffect> effectDict = new Dictionary<TileBase, StatusEffect>();
    Dictionary<string, StatusEffect> generalEffectDict = new Dictionary<string, StatusEffect>();
    [SerializeField] TileBase spikeUpTile;
    [SerializeField] TileBase pitTile;
    public Dictionary<TileBase, StatusEffect> createTileEffectDict()
    {
        BaseEffect[] hurtfxs = new BaseEffect[1];
        hurtfxs[0] = new HurtEffect(100, 0);
        StatusEffect hurtEffect = new StatusEffect(0, hurtfxs, null, null);
        effectDict.Add(spikeUpTile, hurtEffect);
        return effectDict;
    }

    public Dictionary<TileBase, StatusEffect> getEffectDict()
    {
        if(effectDict.Count <= 0) return createTileEffectDict();
        return effectDict;
    }

    public Dictionary<string, StatusEffect> createGeneralEffectDict()
    {
        BaseEffect[] dmgUpFxs = new BaseEffect[1];
        dmgUpFxs[0] = new DamageBuff(100);
        StatusEffect damagePotionEffect = new StatusEffect(50, dmgUpFxs, null, null);
        generalEffectDict.Add("DamagePotion", damagePotionEffect);
        return generalEffectDict;
    }

    public Dictionary<string, StatusEffect> getGeneralEffectDict()
    {
        if (effectDict.Count <= 0) return createGeneralEffectDict();
        return generalEffectDict;
    }
}
