using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseEffect
{
    public enum effectTag { Attack, Health }
    public effectTag tag;
    public int duration;
    virtual public void ApplyEffect(GameObject target) { }
    virtual public void onEffectRemove(GameObject target) { }


}

[Serializable]
public class HurtEffect : BaseEffect
{
    [SerializeField] int damageAmount;
    [SerializeField] int intervals;

    public HurtEffect(int dmg, int inter)
    {
        tag = effectTag.Health;
        damageAmount = dmg;
        intervals = inter;
    }
    public override void ApplyEffect(GameObject target)
    {
        //damage;
        Health targetH = target.GetComponent<Health>();
        if (targetH == null)
        {
            Debug.LogError("Tried to apply a hurt effect to a gameobject without a health component!");
            return;
        }
        else
        {
            targetH.TakeDamage(damageAmount);
        }
    }
    public override int GetHashCode()
    {
        return damageAmount.GetHashCode() + intervals.GetHashCode() + duration.GetHashCode();
    }
}

[Serializable]
public class DamageBuff : BaseEffect
{
    int buffAmount; // is a percent. e.g. 100 is 100% damage boost
    public DamageBuff(int buffAmount)
    {
        tag = effectTag.Attack;
    }

    
    public override void ApplyEffect(GameObject target)
    {
        //buff existing in the manager counts as applied?
    }

    public override int GetHashCode()
    {
        return buffAmount.GetHashCode() + duration.GetHashCode();
    }

}

