using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum damageTypes
{
    FIRE,//You're on fire
    BLUNT,//Someone clubs you
    PIERCING,
    MOMENTUM,//Like you fall 20m that kind
    MAGIC_GENERAL,
    MAGIC_FIRE,
    MAGIC_ICE
    //Can expand...
}
public class DamageEvent
{
    public float damageAmount = 1f;
    public damageTypes damageType;
    public GameObject attacker;
    public GameObject target;
    public float knockbackValue = 1f;
    

    public DamageEvent(float damageAmount, damageTypes damageType, GameObject attacker, GameObject target, float knockbackValue)
    {
        this.damageAmount = damageAmount;
        this.damageType = damageType;
        this.attacker = attacker;
        this.target = target;
        this.knockbackValue = knockbackValue;
        target.GetComponent<IDamageable>().recceiveDamage(this);
    }

    public DamageEvent(float damageAmount, damageTypes damageType, GameObject attacker, GameObject target)
    {
        this.damageAmount = damageAmount;
        this.damageType = damageType;
        this.attacker = attacker;
        this.target = target;
        target.GetComponent<IDamageable>().recceiveDamage(this);
    }
    


}



public interface IDamageable
{
    void recceiveDamage(DamageEvent damageEvent);
}
