using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.General; 
namespace Laserbean.Hitbox2D

{
public class NewAttackController : MonoBehaviour
{
    [SerializeField] AttackInfoObject attackInfoObject;

    public delegate void DisenableBoolDelegate(bool canMove);
    public delegate void MovementDelegate(Vector3 move); 

    public DisenableBoolDelegate DisenableMovement;
    public DisenableBoolDelegate DisenableRotation;

    public MovementDelegate DoMovement; 

    public void SetAttackInfo(AttackInfoObject attt) {
        attackInfoObject = attt; 
        UpdateHitboxes(); 
    }


    // protected void Start() {
    //     if (attackInfoObject == null) return; 
    //     CreateHitboxes(); 
    // }
    [SerializeField] List<string> tag_blacklist = new List<string>(); 

    GameObject prepHGO; 
    HitboxesController prepHitboxesController;
    List<GameObject> attackHGOs = new List<GameObject>();
    List<HitboxesController> attackHitboxesControllers = new List<HitboxesController>();
    GameObject cooldownHGO;
    HitboxesController cooldownHitboxesController;

    void CreateHitboxes() {
        if (attackInfoObject == null) return; 

        prepHGO = new GameObject("prep_hitbox"); 
        prepHGO.transform.SetParent(this.transform); 
        prepHGO.transform.localPosition = Vector3.zero; 

        prepHitboxesController = prepHGO.AddComponent<HitboxesController>(); 
        prepHitboxesController.SetHitbox(attackInfoObject.attack.prep_hitbox); 

        attackHGOs.Clear(); 
        attackHitboxesControllers.Clear(); 
        int i = 0; 
        foreach(HitboxInfo hitbox in attackInfoObject.attack.hitboxes) {
            GameObject go = new GameObject("main_hitbox " + i); 
            go.transform.SetParent(this.transform); 
            go.transform.localPosition = Vector3.zero; 

            i+=1; 
            HitboxesController hitboxcont = go.AddComponent<HitboxesController>(); 

            hitboxcont.SetHitbox(hitbox); 

            attackHGOs.Add(go); 
            attackHitboxesControllers.Add(hitboxcont); 
        }

        cooldownHGO = new GameObject("cooldown"); 
        cooldownHGO.transform.SetParent(this.transform); 
        cooldownHGO.transform.localPosition = Vector3.zero; 

        cooldownHitboxesController = cooldownHGO.AddComponent<HitboxesController>(); 
        cooldownHitboxesController.SetHitbox(attackInfoObject.attack.after_hitbox); 

        canAttack = true;

        UpdateBlacklist();
    }

    void DestroyHitboxes() {
        Destroy(prepHGO); prepHitboxesController = null; 
        Destroy(cooldownHGO); cooldownHitboxesController = null;

        GameObject[] toDelete = attackHGOs.ToArray(); 

        for (int i = toDelete.Length -1; i >= 0; i--) {
            Destroy(toDelete[i]); 
        }
        attackHitboxesControllers.Clear(); 
    }

    public void UpdateHitboxes() {
        DestroyHitboxes(); 
        CreateHitboxes(); 
        // UpdateBlacklist(); 
    }

    public void AddBlacklist(string tagg) {
        if (tag_blacklist.Contains(tagg)) return; 
        tag_blacklist.Add(tagg); 

        UpdateBlacklist(); 
    }

    void UpdateBlacklist() {
        foreach(var thing in tag_blacklist) {
            AddToBlacklist(thing);
        }
    }

    void AddToBlacklist(string tagg) {
        prepHitboxesController?.AddBlacklist(tagg); 
        foreach(var thing in attackHitboxesControllers) {
            thing.AddBlacklist(tagg);
        }
        cooldownHitboxesController?.AddBlacklist(tagg); 
    }

    protected bool InRange(Vector3 enemyPos) {
        return (enemyPos - this.transform.position).magnitude < attackInfoObject.attack.range.x; //TODO
    }



    public bool canAttack = true; 

    public void startAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!canAttack) return; 

        if (attackInfoObject == null) return; 

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfoObject.attack.lock_movement_while_attack) {
            if (DisenableMovement != null) DisenableMovement(false);
        }

        if (attackInfoObject.attack.lock_rotation_while_attack) {
            if (DisenableRotation != null) DisenableRotation(false);
        }

        Vector3 curpos = this.transform.position; 

        // yield return prepHitboxesController.Attack(angle, curpos);
        //TODO maybe add the pos here
        prepHitboxesController.Attack(angle);
        if (prepHitboxesController.hitbox.isBody && DoMovement != null) DoMovement(prepHitboxesController.hitbox.bodymove.Rotate(angle));
        yield return new WaitForSeconds(prepHitboxesController.hitbox.duration);


        
        foreach(var thing in attackHitboxesControllers) {
            thing.Attack(angle, attackInfoObject.attack.max_angle_error); 
            if (thing.hitbox.isBody && DoMovement != null) DoMovement(thing.hitbox.bodymove.Rotate(angle));

            yield return new WaitForSeconds(thing.hitbox.lifetime);
            // yield return new WaitForSeconds(attackInfoObject.attack.attackDelay);
        }

        if (DisenableMovement != null) DisenableMovement(true);
        if (DisenableRotation != null) DisenableRotation(true);

        cooldownHitboxesController.Attack(angle); 
        if (cooldownHitboxesController.hitbox.isBody && DoMovement != null) DoMovement(cooldownHitboxesController.hitbox.bodymove.Rotate(angle));
        yield return new WaitForSeconds(cooldownHitboxesController.hitbox.duration);


        canAttack = true; 
        yield break; 
    }
}
}