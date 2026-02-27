using UnityEngine;

public class EffectEntry
{
    public enum Tag {Attack, Health}
    public StatusEffect effect;
    public float timeLeft;
    public Sprite icon;
    public Tag effectTag;

    public EffectEntry(StatusEffect Ieffect, float duration, Sprite Iicon)
    {
        effect = Ieffect;
        timeLeft = duration;
        icon = Iicon;
    }

}
