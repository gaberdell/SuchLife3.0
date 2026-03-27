using TMPro;
using UnityEngine;

public class EffectEntry
{
    public StatusEffect effect;
    public float timeLeft;
    public Sprite icon;
    public BaseEffect.effectTag effectTag;
    public GameObject effectPanelInScene;
    public TextMeshProUGUI timer;

    public EffectEntry(StatusEffect Ieffect, Sprite Iicon, GameObject panel)
    {
        effect = Ieffect;
        timeLeft = Ieffect.getDuration();
        icon = Iicon;
        effectTag = Ieffect.getEffectTag();
        effectPanelInScene = panel;
        timer = effectPanelInScene.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
    }

}
