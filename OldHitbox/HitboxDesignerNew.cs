using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

// namespace Laserbean.Hitbox2D
// {
// public class HitboxDesignerNew : SingleAttackingEntity
// {

//     [EasyButtons.Button]
//     protected void OnReload() {
//         UpdateHitboxes(); 
//     }

//     [SerializeField] float angle = 0f; 
//     [EasyButtons.Button]
//     void OnFire() {
//         StartAttack(angle);
//     }


    

// }
// }
// #if UNITY_EDITOR

// [ExecuteInEditMode]
// public class HitboxDesignerNew : MonoBehaviour
// {   
//     public AttackInfoObject attackObject; 

//     public GameObject sprite;


//     public BoxCollider2D boxcol;
//     public CircleCollider2D circlecol;
//     public PolygonCollider2D polyconcol; 

//     public void OnValidatee() {
//         OnValidate(); 
//     }

//     private void OnValidate() {
//         if (attackObject == null) return; 
//         // sprite.transform.localPosition = attackObject.attack.hitbox.offset;
//         UpdateStuff(attackObject.attack.hitboxes[0]); 
//         this.transform.localPosition = attackObject.attack.hitboxes[0].local_position; 
//         // sprite.GetComponent<SpriteRenderer>().sprite = attackObject.attack.sprite; 
//     }

//     void UpdateStuff(HitboxInfo2 hitbox) {
//             sprite.GetComponent<SpriteRenderer>().sprite = hitbox.sprites[0]; 

//         boxcol.enabled = false; 
//         polyconcol.enabled = false; 
//         circlecol.enabled = false; 

//         if (hitbox.shape == HitboxShape.Rectangle){
//             boxcol.offset = hitbox.offset; 
//             boxcol.size = hitbox.size; 
//             boxcol.isTrigger = true; 
//             boxcol.enabled = true; 

//         } else if (hitbox.shape == HitboxShape.Circle) {
//             circlecol.offset = hitbox.offset; 
//             circlecol.radius = hitbox.size[0]; 
//             circlecol.isTrigger = true; 
//             circlecol.enabled = true; 
//         } else if (hitbox.shape == HitboxShape.Sector) {
//             polyconcol.GenerateSectorCollider(hitbox.size[1], 90f - hitbox.size[1]/2, hitbox.size[0], hitbox.size[0]/10, 4);
//             polyconcol.offset = hitbox.offset; 
//             polyconcol.isTrigger = true; 
//             polyconcol.enabled = true; 

//         }


//     }

//     private void OnDrawGizmos() {
//         if (attackObject == null) return; 

//         HitboxInfo2 hitbox = attackObject.attack.hitboxes[0];
//         if (hitbox.shape == HitboxShape.Rectangle){  
//             boxcol.Draw(Color.green, this.transform);
//         } else if (hitbox.shape == HitboxShape.Circle) {
//             circlecol.Draw(Color.green, this.transform );
//         } else if (hitbox.shape == HitboxShape.Sector) {
//             polyconcol.Draw(Color.green, this.transform);
//         }
        
//     }
// }



// // [CustomEditor(typeof(HitboxDesigner))]
// // public class HitboxDesignerEditor : Editor
// // {
// //     public override void OnInspectorGUI()
// //     {
// //         DrawDefaultInspector();

// //         HitboxDesigner component = (HitboxDesigner)target;

// //         EditorGUI.BeginChangeCheck();

// //         // // Draw the scriptable object field
// //         // component.itemObject = (ItemObject)EditorGUILayout.ObjectField("Item Object", component.itemObject, typeof(ItemObject), false);

// //         // Check if the field has changed
// //         if (EditorGUI.EndChangeCheck())
// //         {
// //             // The field has changed, so call OnValidate
// //             component.OnValidatee();
// //         }
// //     }
// // }


// #endif