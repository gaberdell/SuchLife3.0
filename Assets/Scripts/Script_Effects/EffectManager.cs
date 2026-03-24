using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //public static EffectManager instance { get; private set; }

    /*
     * This class keeps tracks of effects applied to the player for the purposes of handling as well as displaying status effects.
     * every player (local/online) will have their own EffectManager script
     * Effects are tracked with an EffectEntry class
     */

    public List<EffectEntry> effects = new List<EffectEntry>();
    private GameObject Player;
    bool isLocalPlayer = false; //whether this effect manager is on the local player or not; local player effects will be displayed on the UI

    public void addEffect(StatusEffect effect, Sprite icon)
    {
        EffectEntry newEffectEntry = new EffectEntry(effect, icon);
        effect.setTarget(Player); //any status effects added to the effect manager on a player/entity are necessarily targeting that player/entity.
        effects.Add(newEffectEntry);
        //create ui component from prefab and add it to the scene
    }
    private void Awake()
    {
        //instance = this;
        Player = this.gameObject;
        DontDestroyOnLoad(gameObject);
    }

    
    void FixedUpdate()
    {
        //decrease the timer of effects
        for (int i = 0; i < effects.Count; i++)
        {
            EffectEntry effect = effects[i];
            effect.timeLeft -= Time.deltaTime;
            if (effect.timeLeft < 0)
            {
                effects.Remove(effect);
                i--;
                Debug.Log("effect removed with tag " + effect.effectTag.ToString());
            }
        }

        if (isLocalPlayer)
        {
            //handle ui functions
        }

    }

    public List<EffectEntry> getEffectsWithTag(BaseEffect.effectTag tag)
    {
        List<EffectEntry> r = new List<EffectEntry>();
        foreach(EffectEntry effect in effects)
        {
            if(effect.effectTag == tag) r.Add(effect);
        }
        return r;
    }
}
