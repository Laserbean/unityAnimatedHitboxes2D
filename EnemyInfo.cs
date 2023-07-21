using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EnemyInfo {
    public string Name;
    public int hp;

    public float mass;
    public float linear_drag;
    public float angular_drag; 

    public List<AttackInfoObjectOld> attack_object_list;

}

