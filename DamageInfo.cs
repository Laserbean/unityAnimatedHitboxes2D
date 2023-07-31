using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Laserbean.Hitbox2D
{
[System.Serializable]
public class DamageInfo {
    [System.Serializable]
    public class StatusEffectDuration {
        public StatusEffectObject statusEffect; 
        public float duration;           
    }


    public int damage; 
    public float knockback; 
    public float stun; 
    public float critical; 


    public List<StatusEffectDuration> allStatusEffects = new List<StatusEffectDuration>(); 


    public DamageInfo(int dam, float knock, float stu, float crit) {
        critical = crit; 
        damage = dam; 
        knockback = knock;
        stun = stu;
        // debuff = new StatusEffect{type = DebuffType.None, duration = 0f, rate = 0f}; 

    }

    public DamageInfo(int dam, float knock, float stu, float crit, StatusEffectObject statusObject) {
        critical = crit; 

        damage = dam; 
        knockback = knock;
        stun = stu;
        // statusEffect = statusObject; //FIXME
    }

}
}