using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 

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

    AttackController attackController; 


    protected void UpdateHitboxes() {
        attackController.SetAttackInfo(attackInfoObject); 
    }

    protected void Start() {
        attackController = this.gameObject.AddComponent<AttackController>(); 
        attackController.DisenableMovement = DisenableMovement; 
        attackController.SetAttackInfo(attackInfoObject);

        // if (attackInfoObject == null) return; 
    }

    [SerializeField] List<string> tag_blacklist = new List<string>(); 


    protected void AddBlacklist(string tagg) {
        if (tag_blacklist.Contains(tagg)) return; 

        tag_blacklist.Add(tagg); 
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
