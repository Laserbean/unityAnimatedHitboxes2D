using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 

using Laserbean.Hitbox2D; 

// [System.Obsolete("Use the Attack controller directly maybe")]
public abstract class AttackingEntity : MonoBehaviour
{
    public AttackInfoObject attackInfoObject;

    // public AttackInfo2 attackInfo; 

    public abstract void DisenableMovement(bool canMove);

    #if UNITY_EDITOR
    public void AttackInfoUpdated() {
        UpdateHitboxes();
    }
    #endif

    NewAttackController attackController; 


    protected void UpdateHitboxes() {
        attackController.SetAttackInfo(attackInfoObject); 
    }

    protected void Start() {
        attackController = this.gameObject.AddComponent<NewAttackController>(); 
        attackController.DisenableMovement = DisenableMovement; 
        attackController.SetAttackInfo(attackInfoObject);

        // if (attackInfoObject == null) return; 
    }



    protected void AddBlacklist(string tagg) {
        attackController.AddBlacklist(tagg); 
    }


    protected bool InRange(Vector3 enemyPos) {
        return (enemyPos - this.transform.position).magnitude < attackInfoObject.attack.range.x; 
    }


    bool canAttack = true; 

    protected void startAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!attackController.canAttack) return; 

        attackController.startAttack(angle);

    }


}
