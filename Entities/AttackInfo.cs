using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Laserbean.AttackHitbox2D

{
[System.Serializable]
public class AttackInfo2 {
    public bool lock_movement_while_attack = false; 
    public bool lock_rotation_while_attack = false; 

    [Header ("Prep")] 
    public AttackHitboxInfo prep_hitbox; 
    
    [Header ("Main Attack")] 
    public List<AttackHitboxInfo> hitboxes; 
    public float attackDelay; 

    [Header("Cooldown/reload")]
    public AttackHitboxInfo after_hitbox; 


    [Header("Aim and range and other stuff")]
    public float max_angle_error;

    [Tooltip("[Min, Max, Centre]")]
    public Vector3 range; //min, max, center;
    
    public float reload_time = 0f;

    private float _attack_duration = -1; 

    public float attack_duration {
        get {
            float total = 0f; 
            if (_attack_duration == -1) {
                foreach(var hitbox in hitboxes) {
                    total += hitbox.duration; 
                }
                _attack_duration = total; 
            }
            return _attack_duration; 
        }

    }

}


[System.Serializable]
public class AttackInfo {
    public bool isBody = false; 

    [Header ("Prep")] 
    public AttackHitboxInfo prep_hitbox; 
    
    [Header ("Main Attack")] 
    public AttackHitboxInfo main_hitbox; 

    [Header("Cooldown/reload")]
    public AttackHitboxInfo after_hitbox; 


    [Header("Aim and range and other stuff")]
    public float max_angle_error;

    [Tooltip("[Min, Max, Centre]")]
    public Vector3 range; //min, max, center;
    
    public float reload_time = 0f;


    public float attack_duration {
        get {
            return main_hitbox.duration; 
        }
    }
}
}
