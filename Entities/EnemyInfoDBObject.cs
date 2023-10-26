using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Laserbean.AttackHitbox2D
{
[CreateAssetMenu(fileName = "EnemyInfoDatabase",menuName = "LB Hitbox2D/EnemyInfoDatabase")]
public class EnemyInfoDBObject : ScriptableObject
{
    public List<EnemyInfoObject> list; 


    private void OnValidate() {
        int i = 0; 
        foreach(var thing in list) {
            thing.id = i; 
            i++; 
        }
    }
}

}