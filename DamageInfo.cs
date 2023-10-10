using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Laserbean.Hitbox2D
{

[System.Serializable]
public class StatusEffectDuration {
    public StatusEffectTObject statusEffect; 
    public float duration;    

    public StatusEffectDuration(StatusEffectTObject statusobject, float dur) {
        statusEffect = statusobject;
        duration = dur; 
    }       
}


[System.Serializable]
public class DamageInfo {
    public int damage_ammount; 
    public float knockback; 
    public float stun; 
    public float critical; 

    public List<StatusEffectDuration> allStatusEffects = new (); 

    public WeaponType weaponType; 
    public DamageType damageType; 

    public DamageInfo(int dam, float knock, float stu, float crit) {
        critical = crit; 
        damage_ammount = dam; 
        knockback = knock;
        stun = stu;
        // debuff = new StatusEffect{type = DebuffType.None, duration = 0f, rate = 0f}; 

    }

    // public DamageInfo(int dam, float knock, float stu, float crit, StatusEffectTObject statusObject) {
    //     critical = crit; 

    //     damage_ammount = dam; 
    //     knockback = knock;
    //     stun = stu;
    //     // statusEffect = statusObject; //FIXME
    // }

    public Damage GetDamage(Vector2 normalized_direction) {
        int dmage = UnityEngine.Random.Range(0f, 1f) > critical ? damage_ammount * 2 : damage_ammount;
        var dmg = new Damage(dmage, normalized_direction * knockback, stun)
        {
            allStatusEffects = new(allStatusEffects),
            weaponType = weaponType,
            damageType = damageType
        };


            return dmg; 
    }

}

public struct Damage {
    public int damage; 
    public Vector3 knockback; 
    public float stun;

    public List<StatusEffectDuration> allStatusEffects; 

    public Damage(int dmm, Vector3 knock, float stn) {
        damage = dmm;
        knockback = knock; 
        stun = stn; 
        
        allStatusEffects =  new ();

        weaponType = WeaponType.Nothing; 
        damageType = DamageType.Physical; 
    }

    public WeaponType weaponType; 
    public DamageType damageType;
}

public enum WeaponType {
    Nothing,
    Melee,
    Bullet,
    Magic
}

public enum DamageType {
    Nothing,
    Physical,
    Infected,
    Fire,
    Water,
    Ice,
    Light,
    Dark,
    Poison,
    
}


[System.Serializable]
public class DamageModifier
{
    [SerializeField]
    float damageMultiplier;
    [SerializeField]
    DamageType newDamageType;

    [SerializeField]
    StatusEffectDuration statusEffectDuration; 


    public DamageModifier(float multiplier, DamageType damageType, StatusEffectTObject StatusEffectTObject, float dur) {
        damageMultiplier = multiplier; 
        newDamageType = damageType; 
        statusEffectDuration = new(StatusEffectTObject, dur); 
    }

    public void ModifyDamage(ref Damage cur_damage)
    {

        cur_damage.damage = Mathf.RoundToInt(cur_damage.damage * damageMultiplier);

        if (newDamageType != DamageType.Nothing)
            cur_damage.damageType = newDamageType;

        if (statusEffectDuration.duration > 0) 
            cur_damage.allStatusEffects.Add(statusEffectDuration);

    }
}

}