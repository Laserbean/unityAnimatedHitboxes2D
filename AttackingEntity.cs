using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 

namespace Laserbean.Hitbox2D
{


public interface IAttackingEntity {

    public List<AttackInfoObject> GetAttackInfoObjects(); 
    public void AttackInfoUpdated();

    void UpdateHitboxes();
    void AddBlacklist(string tagg);
    bool InRange(Vector3 enemyPos, int num = 0);
    void StartAttack(float angle, int num = 0);
}



    [System.Serializable] 
    public class AttackSet {

        [HideInInspector]
        public GameObject attackObject; 
        public AttackInfoObject attackInfoObject; 

        [HideInInspector]
        public NewAttackController attackController; 


        public void Initialize(int num, List<string> blacklisttag, NewAttackController.DisenableBoolDelegate disenablemovement, 
        NewAttackController.DisenableBoolDelegate disenablerotation, 
        NewAttackController.MovementDelegate movement,
        Transform parent) {
            attackObject = new GameObject("attackobject" + num); 
            attackObject.transform.SetParent(parent);
            attackObject.transform.localPosition = Vector3.zero; 


            attackController = attackObject.AddComponent<NewAttackController>(); 
            attackController.DisenableMovement = disenablemovement; 
            attackController.DisenableRotation = disenablerotation; 

            attackController.DoMovement = movement; 
            attackController.SetAttackInfo(attackInfoObject);


            foreach(var tag in blacklisttag) {
                attackController.AddBlacklist(tag); 
            }


        }

        public void UpdateSet() {
            if (attackObject == null) return; 
            if (attackController == null) attackController = attackObject.GetComponent<NewAttackController>(); 
            attackController.UpdateHitboxes(); 
        }

        
        public bool InRange(float distance) {
            return distance < attackInfoObject.attack.range.y && distance > attackInfoObject.attack.range.x; 
        }

        
    }


}



