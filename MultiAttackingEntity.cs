using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 

using UnityEngine.Events;


namespace Laserbean.Hitbox2D
{
// [System.Obsolete("Use the Attack controller directly maybe")]
public class MultiAttackingEntity : MonoBehaviour, IAttackingEntity
{

    [System.Serializable] 
    public class AttackSet {

        public GameObject attackObject; 
        public AttackInfoObject attackInfoObject; 

        [HideInInspector]
        public NewAttackController attackController; 


        public void Initialize(int num, List<string> blacklisttag, NewAttackController.DisenableMovementDelegate disenablemovement, 
        NewAttackController.MovementDelegate movement,
        Transform parent) {
            attackObject = new GameObject("attackobject" + num); 
            attackObject.transform.SetParent(parent);
            attackObject.transform.localPosition = Vector3.zero; 


            attackController = attackObject.AddComponent<NewAttackController>(); 
            attackController.DisenableMovement = disenablemovement; 
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

    public List<AttackSet> attackSets; 


    [SerializeField] List<string> BlackListTags = new List<string>(); 

    public void DisenableMovement(bool canMove) {
        disenableEvent.Invoke(canMove); 
    }

    public void DoMovement(Vector2 move) {
        moveEvent.Invoke(move); 
    }


    // public void UnityEvent

    public MyBoolEvent disenableEvent; 

    public MyVector2Event moveEvent; 

    #if UNITY_EDITOR
    public void AttackInfoUpdated() {
        UpdateHitboxes();
    }
    #endif


    protected void UpdateHitboxes() {

        foreach(var attackset in attackSets) {
            attackset.UpdateSet();
        }


    }

    protected void Start() {
        int i = 0;
        foreach(var attackset in attackSets) {
            attackset.Initialize(i, BlackListTags, DisenableMovement, DoMovement, this.transform);
        }

        // if (attackInfoObject == null) return; 
    }



    protected bool InRange(Vector3 enemyPos, int curattack) {

        float distance = (enemyPos - this.transform.position).magnitude; 
        return attackSets[curattack].InRange(distance); 
    }


    bool canAttack = true; 

    protected void startAttack(float angle, int attacknum = 0) {
        if(!GameManager.Instance.IsRunning) return; 

        if (attacknum >= attackSets.Count) return; 

        AttackSet curset = attackSets[attacknum];

        if (!curset.attackController.canAttack) return; 
        curset.attackController.startAttack(angle);
    }

    List<AttackInfoObject> IAttackingEntity.GetAttackInfoObjects()
    {
        List<AttackInfoObject> list = new List<AttackInfoObject>();

        foreach(var attackset in attackSets) {
            list.Add(attackset.attackInfoObject); 
        }

        return list; 
    }

}

}

