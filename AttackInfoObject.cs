using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackInfo", menuName = "Attack/Attack Info")]
public class AttackInfoObject : ScriptableObject
{


    [EasyButtons.Button]
    protected virtual void BUTTON(){

        #if UNITY_EDITOR 
            AttackingEntity[] components = Resources.FindObjectsOfTypeAll<AttackingEntity>();
            foreach (AttackingEntity component in components)
            {
                if (component.attackInfoObject == this)
                {
                    component.AttackInfoUpdated();
                }
            }

        #endif
    }

    public AttackInfo2 attack; 


}
