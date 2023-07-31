using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laserbean.Hitbox2D
{
[CreateAssetMenu(fileName = "AttackInfo", menuName = "LB Hitbox2D/Attack Info")]
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

}