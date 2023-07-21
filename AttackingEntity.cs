using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding; 


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

    protected void Start() {
        if (attackInfoObject == null) return; 

        // attackInfo = attackInfoObject.attack; 
        CreateHitboxes(); 
    }
    [SerializeField] List<string> tag_blacklist = new List<string>(); 

    GameObject prepHGO; 
    HitboxController prepHitboxController;
    List<GameObject> attackHGOs = new List<GameObject>();
    List<HitboxController> attackHitboxControllers = new List<HitboxController>();
    GameObject cooldownHGO;
    HitboxController cooldownHitboxController;

    protected void CreateHitboxes() {
        prepHGO = HitboxController.CreateHitbox(this.transform, attackInfoObject.attack.prep_hitbox, "prep"); 
        prepHitboxController = prepHGO.GetComponent<HitboxController>(); 


        attackHGOs.Clear(); 
        attackHitboxControllers.Clear(); 
        foreach(HitboxInfo2 hitbox in attackInfoObject.attack.hitboxes) {
            GameObject go = HitboxController.CreateHitbox(this.transform, hitbox, "mainattack");
            HitboxController hitboxcont = go.GetComponent<HitboxController>(); 
            attackHGOs.Add(go); 
            attackHitboxControllers.Add(hitboxcont); 
        }

        cooldownHGO = HitboxController.CreateHitbox(this.transform, attackInfoObject.attack.after_hitbox, "cooldown"); 
        cooldownHitboxController = cooldownHGO.GetComponent<HitboxController>(); 

        canAttack = true;

        UpdateBlacklist();
    }

    void DestroyHitboxes() {
        Destroy(prepHGO); prepHitboxController = null; 
        Destroy(cooldownHGO); cooldownHitboxController = null;

        GameObject[] toDelete = attackHGOs.ToArray(); 

        for (int i = toDelete.Length -1; i >= 0; i--) {
            Destroy(toDelete[i]); 
        }
        attackHitboxControllers.Clear(); 



    }

    public void UpdateHitboxes() {
        // while (transform.childCount > 0) {
        //     DestroyImmediate(transform.GetChild(0).gameObject);
        // }
        DestroyHitboxes(); 
        CreateHitboxes(); 


        // prepHitboxController.SetupHitbox(attackInfoObject.attack.prep_hitbox, this.transform);

        // for (int i = 0; i < attackInfoObject.attack.hitboxes.Count; i++) {
        //     attackHitboxControllers[i].SetupHitbox(attackInfoObject.attack.hitboxes[i], this.transform);
        // }
        // cooldownHitboxController.SetupHitbox(attackInfoObject.attack.after_hitbox, this.transform);

    }

    protected void AddBlacklist(string tagg) {
        if (tag_blacklist.Contains(tagg)) return; 

        tag_blacklist.Add(tagg); 
    }

    void UpdateBlacklist() {
        foreach(var thing in tag_blacklist) {
            AddToBlacklist(thing);
        }
    }

    void AddToBlacklist(string tagg) {
        prepHitboxController.AddBlacklist(tagg); 
        foreach(var thing in attackHitboxControllers) {
            thing.AddBlacklist(tagg);
        }
        cooldownHitboxController.AddBlacklist(tagg); 
    }


    protected bool InRange(Vector3 enemyPos) {
        return (enemyPos - this.transform.position).magnitude < attackInfoObject.attack.range.x; 
    }


    bool canAttack = true; 

    protected void startAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!canAttack) return; 

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfoObject.attack.isBody) {
            DisenableMovement(false);
        }

        Vector3 curpos = this.transform.position; 

        // prepHitboxController.Attack(angle, curpos); 
        // yield return new WaitForSeconds(attackInfoObject.attack.prep_hitbox.duration);

        yield return prepHitboxController.Attack(angle, curpos); 

        
        foreach(var thing in attackHitboxControllers) {
            yield return thing.Attack(angle,curpos); 
        }

        // yield return new WaitForSeconds(attackInfoObject.attack.attack_duration);
        DisenableMovement(true);
        yield return cooldownHitboxController.Attack(angle, curpos); 
        // yield return new WaitForSeconds(attackInfoObject.attack.after_hitbox.duration);

        canAttack = true; 
        yield break; 
    }
}
