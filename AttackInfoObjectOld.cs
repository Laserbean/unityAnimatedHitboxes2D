using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackInfoOld", menuName = "Enemies/Attack Info Old")]
public class AttackInfoObjectOld : ScriptableObject
{
    public AttackInfoOld attack; 


    protected virtual void OnValidate(){

        #if UNITY_EDITOR 
        // Debug.Log("Validate item object: "+ name); 
           
            // Notify any attached components of changes to this item object's fields
            HitboxDesigner[] components = Resources.FindObjectsOfTypeAll<HitboxDesigner>();
            foreach (HitboxDesigner component in components)
            {
                if (component.attackObject == this)
                {
                    component.OnValidatee();
                }
            }
        // EditorUtility.SetDirty(this); 


        #endif
    }

}
