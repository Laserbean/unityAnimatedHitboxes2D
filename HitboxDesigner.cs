using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


#if UNITY_EDITOR

[ExecuteInEditMode]
public class HitboxDesigner : MonoBehaviour
{   
    public AttackInfoObjectOld attackObject; 

    public GameObject sprite;


    public BoxCollider2D boxcol;
    public CircleCollider2D circlecol;
    public PolygonCollider2D polyconcol; 

    public void OnValidatee() {
        OnValidate(); 
    }

    private void OnValidate() {
            // sprite.transform.localPosition = attackObject.attack.hitbox.offset;
            if(attackObject == null)return; 
            UpdateStuff(attackObject.attack); 
            this.transform.localPosition = attackObject.attack.hitbox.local_position; 
            sprite.GetComponent<SpriteRenderer>().sprite = attackObject.attack.sprite; 
    }

    void UpdateStuff(AttackInfoOld attack) {
            sprite.GetComponent<SpriteRenderer>().sprite = attackObject.attack.sprite; 

        boxcol.enabled = false; 
        polyconcol.enabled = false; 
        circlecol.enabled = false; 

        if (attack.hitbox.shape == HitboxShape.Rectangle){
            boxcol.offset = attack.hitbox.offset; 
            boxcol.size = attack.hitbox.size; 
            boxcol.isTrigger = true; 
            boxcol.enabled = true; 

        } else if (attack.hitbox.shape == HitboxShape.Circle) {
            circlecol.offset = attack.hitbox.offset; 
            circlecol.radius = attack.hitbox.size[0]; 
            circlecol.isTrigger = true; 
            circlecol.enabled = true; 
        } else if (attack.hitbox.shape == HitboxShape.Sector) {
            polyconcol.GenerateSectorCollider(attack.hitbox.size[1], 90f - attack.hitbox.size[1]/2, attack.hitbox.size[0], attack.hitbox.size[0]/10, 4);
            polyconcol.offset = attack.hitbox.offset; 
            polyconcol.isTrigger = true; 
            polyconcol.enabled = true; 

        }


    }

    private void OnDrawGizmos() {
        if(attackObject == null)return; 

        AttackInfoOld attack = attackObject.attack;
        if (attack.hitbox.shape == HitboxShape.Rectangle){  
            boxcol.Draw(Color.green, this.transform);
        } else if (attack.hitbox.shape == HitboxShape.Circle) {
            circlecol.Draw(Color.green, this.transform );
        } else if (attack.hitbox.shape == HitboxShape.Sector) {
            polyconcol.Draw(Color.green, this.transform);
        }
        
    }
}



// [CustomEditor(typeof(HitboxDesigner))]
// public class HitboxDesignerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         HitboxDesigner component = (HitboxDesigner)target;

//         EditorGUI.BeginChangeCheck();

//         // // Draw the scriptable object field
//         // component.itemObject = (ItemObject)EditorGUILayout.ObjectField("Item Object", component.itemObject, typeof(ItemObject), false);

//         // Check if the field has changed
//         if (EditorGUI.EndChangeCheck())
//         {
//             // The field has changed, so call OnValidate
//             component.OnValidatee();
//         }
//     }
// }


#endif