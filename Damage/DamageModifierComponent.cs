using System.Collections;
using System.Collections.Generic;
using Laserbean.General;
using Laserbean.AttackHitbox2D;
using UnityEngine;
using unityInventorySystem.Attribute;

public class DamageModifierComponent : MonoBehaviour, IDamageModify
{

    [SerializeField]
    List<DamageModifier2> damageModifiers = new (); 

    void IDamageModify.ModifyDamage(ref Damage damage)
    {
        Debug.Log("YES this works".DebugColor(Color.magenta)); 

        foreach(var damageMod in damageModifiers) {
            damageMod.ModifyDamage(ref damage);
        }

        int fish = iattributes.GetAttributeValue(AttributeType.Strength);

        damage.damage += fish; 

    }

    IAttributeUsage iattributes; 

    private void Start() {
        iattributes = this.GetComponent<IAttributeUsage>(); 
    }

}
