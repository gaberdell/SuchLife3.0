using UnityEngine;

public class EffectEntry
{
    public StatusEffect effect;
    public float timeLeft;
    public Sprite icon;
    public BaseEffect.effectTag effectTag;

    public EffectEntry(StatusEffect Ieffect, Sprite Iicon)
    {
        effect = Ieffect;
        timeLeft = Ieffect.getDuration();
        icon = Iicon;
        effectTag = Ieffect.getEffectTag();
    }

}
