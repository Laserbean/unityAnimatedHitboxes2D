using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 


public abstract class AtackableEntityOld : MonoBehaviour
{

   

    public AttackInfoObjectOld attackInfoObject;
    public AttackInfoOld attackInfo; 


    GameObject hitboxobject; 



    public abstract void DisenableMovement(bool canMove);


    protected void Start() {
        attackInfo = attackInfoObject.attack; 
        CreateHitbox(); 
    }

    protected void CreateHitbox() {
        hitboxobject = Hitbox_v2.CreateHitbox(this.transform, attackInfo); 
    }

    protected void AddBlacklist(string tagg) {
        hitboxobject.GetComponent<Hitbox_v2>().AddBlacklist(tagg); 
    }


    protected bool InRange(Vector3 enemyPos) {
        return (enemyPos - this.transform.position).magnitude < attackInfoObject.attack.range.x; 
    }


    bool canAttack = true; 

    protected void startAttack(float angle) {
        if (!canAttack) return; 

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfo.isBody) {
            DisenableMovement(false);
        }

        hitboxobject.GetComponent<Hitbox_v2>().Attack(angle); 
        yield return new WaitForSeconds(attackInfo.prep_time);
        yield return new WaitForSeconds(attackInfo.duration);
        DisenableMovement(true);
        yield return new WaitForSeconds(attackInfo.cooldown_time);

        canAttack = true; 
        yield break; 
    }
}
