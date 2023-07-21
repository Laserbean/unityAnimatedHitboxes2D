using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "EnemyInfoDatabase",menuName = "Attack/EnemyInfoDatabase")]
public class EnemyInfoDB : ScriptableObject
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


