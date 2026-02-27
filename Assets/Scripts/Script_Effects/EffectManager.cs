using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance { get; private set; }

    /*
     * This singleton keeps tracks of effects applied to the player for the purposes of handling as well as displaying status effects.
     * Effects are tracked with an EffectEntry class
     */

    public List<EffectEntry> effects = new List<EffectEntry>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    void FixedUpdate()
    {
        //decrease the timer of effects
        foreach (EffectEntry effect in effects)
        {
            effect.timeLeft -= Time.deltaTime;
        }
    }
}
