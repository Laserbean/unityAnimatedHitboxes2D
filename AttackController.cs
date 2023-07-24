using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] AttackInfoObject attackInfoObject;

    public delegate void DisenableMovementDelegate(bool canMove);

    public DisenableMovementDelegate DisenableMovement;

    public void SetAttackInfo(AttackInfoObject attt) {
        attackInfoObject = attt; 
        UpdateHitboxes(); 
    }

    protected void Start() {
        if (attackInfoObject == null) return; 
        CreateHitboxes(); 
    }
    [SerializeField] List<string> tag_blacklist = new List<string>(); 

    GameObject prepHGO; 
    HitboxController prepHitboxController;
    List<GameObject> attackHGOs = new List<GameObject>();
    List<HitboxController> attackHitboxControllers = new List<HitboxController>();
    GameObject cooldownHGO;
    HitboxController cooldownHitboxController;

    void CreateHitboxes() {
        prepHGO = HitboxController.CreateHitbox(this.transform, attackInfoObject.attack.prep_hitbox, "prep"); 
        prepHitboxController = prepHGO.GetComponent<HitboxController>(); 

        attackHGOs.Clear(); 
        attackHitboxControllers.Clear(); 
        foreach(HitboxInfo hitbox in attackInfoObject.attack.hitboxes) {
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

    void UpdateHitboxes() {
        DestroyHitboxes(); 
        CreateHitboxes(); 
        UpdateBlacklist(); 
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
        prepHitboxController.AddBlacklist(tagg); 
        foreach(var thing in attackHitboxControllers) {
            thing.AddBlacklist(tagg);
        }
        cooldownHitboxController.AddBlacklist(tagg); 
    }

    protected bool InRange(Vector3 enemyPos) {
        return (enemyPos - this.transform.position).magnitude < attackInfoObject.attack.range.x; //TODO
    }



    public bool canAttack = true; 

    public void startAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!canAttack) return; 

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfoObject.attack.isBody) {
            if (DisenableMovement != null) DisenableMovement(false);
        }

        Vector3 curpos = this.transform.position; 

        yield return prepHitboxController.Attack(angle, curpos); 

        
        foreach(var thing in attackHitboxControllers) {
            yield return thing.Attack(angle,curpos); 
        }

        DisenableMovement(true);
        yield return cooldownHitboxController.Attack(angle, curpos); 

        canAttack = true; 
        yield break; 
    }
}
