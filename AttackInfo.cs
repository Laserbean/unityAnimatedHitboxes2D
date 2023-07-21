using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class AttackInfo2 {
    public bool isBody = false; 

    [Header ("Prep")] 
    public HitboxInfo2 prep_hitbox; 
    
    [Header ("Main Attack")] 
    public List<HitboxInfo2> hitboxes; 

    [Header("Cooldown/reload")]
    public HitboxInfo2 after_hitbox; 


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