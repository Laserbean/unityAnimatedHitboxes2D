using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "EnemyInfoDatabase",menuName = "Enemies/EnemyInfoDatabase")]
public class EnemyInfoDatabaseOld : ScriptableObject
{
    public List<EnemyInfoObjectOld> list; 


    private void OnValidate() {
        int i = 0; 
        foreach(var thing in list) {
            thing.id = i; 
            i++; 
        }
    }
}


