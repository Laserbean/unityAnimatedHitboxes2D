using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Laserbean.Hitbox2D
{
[System.Serializable]
public class DamageInfo {
    public int damage_ammount; 
    public float knockback; 
    public float stun; 
    public float critical; 

    [System.Serializable]
    public class StatusEffectDuration {
        public StatusEffectObject statusEffect; 
        public float duration;           
    }

    public List<StatusEffectDuration> allStatusEffects = new List<StatusEffectDuration>(); 


    public DamageInfo(int dam, float knock, float stu, float crit) {
        critical = crit; 
        damage_ammount = dam; 
        knockback = knock;
        stun = stu;
        // debuff = new StatusEffect{type = DebuffType.None, duration = 0f, rate = 0f}; 

    }

    public DamageInfo(int dam, float knock, float stu, float crit, StatusEffectObject statusObject) {
        critical = crit; 

        damage_ammount = dam; 
        knockback = knock;
        stun = stu;
        // statusEffect = statusObject; //FIXME
    }

    public Damage damage {
        get {
            int dmage = UnityEngine.Random.Range(0f, 1f) > critical ? damage_ammount * 2 : damage_ammount; 
            return new Damage(dmage, Vector3.zero, stun); 
        }
    }

}

public struct Damage {
    public int damage; 
    public Vector3 knockback; 
    public float stun;

    public Damage(int dmm, Vector3 knock, float stn) {
        damage = dmm;
        knockback = knock; 
        stun = stn; 
    }
}


}