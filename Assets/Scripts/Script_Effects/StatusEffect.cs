using UnityEngine;

//[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
/*  Intended functionality is to be able to create a statuseffect object in the editor, pick its effects, and be able to assign that effect to prefab options.
 *  However, the base unity editor does not support this level of editor configuration (specifically selecting subclasses of BaseEffect to go in a BaseEffect reference)
 *  and would require the implementation of an open source unity package https://github.com/mackysoft/Unity-SerializeReferenceExtensions?tab=readme-ov-file which I am holding back on until approval
 */

//when instantiating statuseffects from scripts for tile behaviors and such just declare the effects then.
public class StatusEffect 
{
    private float duration;
    private Sprite icon;
    private BaseEffect[] effects;
    private GameObject target; //declared when effect is applied
    private BaseEffect.effectTag effectTag; //must match tag of all of its effects
    public StatusEffect(float Iduration, BaseEffect[] fx, Sprite Iicon, GameObject Itarget)
    {
        duration = Iduration;
        icon = Iicon;
        effects = fx;
        target = Itarget;
        effectTag = fx[0].tag;
    }

    public void setTarget(GameObject t)
    {
        target = t;
    }

    public float getDuration()
    {
        return duration;
    }
    public BaseEffect.effectTag getEffectTag()
    {
        return effectTag;
    }

    public void ApplyEffects(GameObject t = null)
    {
        foreach (BaseEffect effect in effects)
        {
            if (t != null) target = t;
            effect.ApplyEffect(target);
        }
    }
}
