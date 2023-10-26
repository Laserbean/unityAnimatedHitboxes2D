using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace Laserbean.AttackHitbox2D
{
// [System.Obsolete("Use the Attack controller directly maybe")]
public class MultiAttackingEntity : MonoBehaviour, IAttackingEntity
{
    public List<AttackSet> attackSets; 
    [SerializeField] List<string> BlackListTags = new List<string>(); 

    public void DisenableMovement(bool canMove) {
        disenableMovementEvent.Invoke(canMove); 
    }

    public void DisenableRotation(bool canMove) {
        disenableRotationEvent.Invoke(canMove); 
    }


    public void DoMovement(Vector3 move) {
        moveEvent.Invoke(move); 
    }

    public MyBoolEvent disenableMovementEvent; 
    public MyBoolEvent disenableRotationEvent; 
    public MyVector3Event moveEvent; 
    
    public class MyFishEvent : UnityEvent {}


    public void SetWeapon(int num, AttackInfoObject attackinfoObject){
        if (num >= attackSets.Count) {
            throw new System.ArgumentOutOfRangeException(); 
        }
        attackSets[num].attackController.SetAttackInfo(attackinfoObject); 
        attackSets[num].UpdateSet();
    }

    public void AttackInfoUpdated() {
        UpdateHitboxes();
    }

    private void OnDisable() {
        UpdateHitboxes();
    }

    public void UpdateHitboxes() {
        foreach(var attackset in attackSets) {
            attackset.UpdateSet();
        }
    }

    void Awake() {
        int i = 0;
        foreach(var attackset in attackSets) {
            attackset.Initialize(i, BlackListTags, DisenableMovement, DisenableRotation, DoMovement, this.transform);
        }
    }

    public int GetRandomAttack()
    {
        return Random.Range(0, attackSets.Count); 
    }


    public bool InRange(Vector3 enemyPos, int curattack = 0) {
        float distance = (enemyPos - this.transform.position).magnitude; 
        return attackSets[curattack].InRange(distance); 
    }


    public void StartAttack(float angle, int attacknum = 0) {
        if(!GameManager.Instance.IsRunning) return; 

        if (attacknum >= attackSets.Count) return; 

        AttackSet curset = attackSets[attacknum];

        if (!curset.attackController.canAttack) return; 
        curset.attackController.StartAttack(angle);
    }

    List<AttackInfoObject> IAttackingEntity.GetAttackInfoObjects() {
        List<AttackInfoObject> list = new ();

        foreach(var attackset in attackSets) {
            list.Add(attackset.attackInfoObject); 
        }

        return list; 
    }


    public void AddBlacklist(string tagg)
    {
        throw new System.NotImplementedException();
    }


    }

}

