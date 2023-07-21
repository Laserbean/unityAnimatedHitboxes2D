using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Attack/EnemyInfo")]
public class EnemyInfoObject : ScriptableObject
{
    public string Name; 
    public int id; 
    public GameObject prefab; 
    public EnemyInfo info;

}

