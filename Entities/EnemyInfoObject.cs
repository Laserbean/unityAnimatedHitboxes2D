using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laserbean.AttackHitbox2D
{
[CreateAssetMenu(fileName = "EnemyInfo", menuName = "LB Hitbox2D/EnemyInfo")]
public class EnemyInfoObject : ScriptableObject
{
    public string Name; 
    public int id; 
    public GameObject prefab; 
    public EnemyInfo info;

}

}