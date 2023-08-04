using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 

namespace Laserbean.Hitbox2D
{
// [System.Obsolete("Use the Attack controller directly maybe")]
public abstract class AttackingEntity : MonoBehaviour, IAttackingEntity
{
    public AttackInfoObject attackInfoObject;

    [SerializeField] List<string> BlackListTags = new List<string>(); 

    public abstract void DisenableMovement(bool canMove);

    #if UNITY_EDITOR
    public void AttackInfoUpdated() {
        UpdateHitboxes();
    }

    List<AttackInfoObject> IAttackingEntity.GetAttackInfoObjects()
    {
        List<AttackInfoObject> list = new List<AttackInfoObject>(); 
        list.Add(attackInfoObject); 
        return list;
    }

    void IAttackingEntity.AttackInfoUpdated()
    {
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

        foreach(var tag in BlackListTags) {
            attackController.AddBlacklist(tag); 
        }

        // if (attackInfoObject == null) return; 
    }



    protected void AddBlacklist(string tagg) {
        attackController.AddBlacklist(tagg); 
    }


    protected bool InRange(Vector3 enemyPos) {
        float distance = (enemyPos - this.transform.position).magnitude; 
        return distance < attackInfoObject.attack.range.y && distance > attackInfoObject.attack.range.x; 
    }


    bool canAttack = true; 

    protected void startAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!attackController.canAttack) return; 

        attackController.startAttack(angle);

    }


}

public interface IAttackingEntity {
    public List<AttackInfoObject> GetAttackInfoObjects(); 
    public void AttackInfoUpdated();
}



}



