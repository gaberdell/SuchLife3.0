using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectManager : MonoBehaviour
{
    //public static EffectManager instance { get; private set; }

    /*
     * This class keeps tracks of effects applied to the player for the purposes of handling as well as displaying status effects.
     * every player (local/online) will have their own EffectManager script
     * Effects are tracked with an EffectEntry class
     */

    /* EFFECT HANDLING RULES
     * A given StatusEffect is only allowed to be present in one EffectEntry at a time; if a status is reapplied for some reason, its duration will be refreshed to its starting value.
     * Multiple StatusEffects with the same tag are allowed to be present simultaneously as long as they have unique StatusEffect classes. (see the class itself for equality definition)
     * 
     */

    public List<EffectEntry> effects = new List<EffectEntry>();
    private GameObject Player;
    [SerializeField] bool isLocalPlayer = false; //whether this effect manager is on the local player or not; local player effects will be displayed on the UI
    [SerializeField] GameObject effectUIPanel; //prefab for effect panel
    [SerializeField] GameObject effectUIParent; //parent holding panel objects in the scene's canvas; should be canvas/GameUI/EffectsList
    public void addEffect(StatusEffect effect, Sprite icon)
    {
        //check if effect is already present in the manager
        if (hasEffect(effect))
        {
            Debug.Log("effect already present; refreshing duration");
            getEffectEntry(effect).timeLeft = effect.getDuration();
            return;
        }

        //create ui component from prefab and add it to the scene
        GameObject effectPanel = Instantiate(effectUIPanel);
        effectPanel.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        effectPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = effect.getDuration().ToString();
        effectPanel.transform.SetParent(effectUIParent.transform, false);
        EffectEntry newEffectEntry = new EffectEntry(effect, icon, effectPanel);
        effect.setTarget(Player); //any status effects added to the effect manager on a player/entity are necessarily targeting that player/entity.
        effects.Add(newEffectEntry);
        
    }

    private bool hasEffect(StatusEffect effect)
    {
        foreach (EffectEntry entry in effects)
        {
            if (entry.effect.Equals(effect)) return true;
        }
        return false;
    }
    private EffectEntry getEffectEntry(StatusEffect effect)
    {
        foreach (EffectEntry entry in effects)
        {
            if (entry.effect.Equals(effect)) return entry;
        }
        //not found in manager
        return null;
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
            if (isLocalPlayer)
            {
                //handle ui functions
                //decrement timer
                effect.timer.text = Mathf.Floor(effect.timeLeft).ToString();

            }
            if (effect.timeLeft < 0)
            {
                Destroy(effect.effectPanelInScene);
                effects.Remove(effect);
                i--;
                Debug.Log("effect removed with tag " + effect.effectTag.ToString());
                
            }
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
