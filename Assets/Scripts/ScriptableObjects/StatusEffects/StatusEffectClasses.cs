using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseEffect
{
    [SerializeField] int duration;
    virtual public void ApplyEffect(GameObject target) { }
}

[Serializable]
public class HurtEffect : BaseEffect
{
    [SerializeField] int damageAmount;
    [SerializeField] int intervals;
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
}

[Serializable]
public class DamageBuff : BaseEffect
{
    [SerializeField] int buffAmount;
    public override void ApplyEffect(GameObject target)
    {
        
    }
}

