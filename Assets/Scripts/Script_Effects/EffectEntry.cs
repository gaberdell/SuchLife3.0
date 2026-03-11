using UnityEngine;

public class EffectEntry
{
    public StatusEffect effect;
    public float timeLeft;
    public Sprite icon;
    public BaseEffect.effectTag effectTag;

    public EffectEntry(StatusEffect Ieffect, float duration, Sprite Iicon, BaseEffect.effectTag t)
    {
        effect = Ieffect;
        timeLeft = duration;
        icon = Iicon;
        effectTag = t;
    }

}
