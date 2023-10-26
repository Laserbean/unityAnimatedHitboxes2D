using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laserbean.AttackHitbox2D
{
[CreateAssetMenu(fileName = "AttackInfo", menuName = "LB Hitbox2D/Attack Info")]
public class AttackInfoObject : ScriptableObject
{
    #if UNITY_EDITOR 

    [EasyButtons.Button]
    protected virtual void BUTTON(){

        // AttackingEntity[] components = Resources.FindObjectsOfTypeAll<AttackingEntity>();
        // foreach (AttackingEntity component in components) {
        //     if (component.attackInfoObject == this) {
        //         component.AttackInfoUpdated();
        //     }
        // }
        FindAndCallFunction<SingleAttackingEntity>(); 
        FindAndCallFunction<MultiAttackingEntity>(); 


    }

    void FindAndCallFunction<T>() where T: UnityEngine.Object, IAttackingEntity
    { 
        T[] components = Resources.FindObjectsOfTypeAll<T>();
        foreach (T component in components) {

            foreach(var attackInfoObject in component.GetAttackInfoObjects()) {
                if (attackInfoObject == this) {
                    component.AttackInfoUpdated();
                }
            }
        }


    }

    #endif


    public AttackInfo2 attack; 


}

}