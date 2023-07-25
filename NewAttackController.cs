using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAttackController : MonoBehaviour
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
    HitboxesController prepHitboxesController;
    List<GameObject> attackHGOs = new List<GameObject>();
    List<HitboxesController> attackHitboxesControllers = new List<HitboxesController>();
    GameObject cooldownHGO;
    HitboxesController cooldownHitboxesController;

    void CreateHitboxes() {
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

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfoObject.attack.isBody) {
            if (DisenableMovement != null) DisenableMovement(false);
        }

        Vector3 curpos = this.transform.position; 

        // yield return prepHitboxesController.Attack(angle, curpos);
        //TODO maybe add the pos here
        prepHitboxesController.Attack(angle);
        yield return new WaitForSeconds(prepHitboxesController.hitbox.duration);


        
        foreach(var thing in attackHitboxesControllers) {
            thing.Attack(angle); 
            yield return new WaitForSeconds(thing.hitbox.duration);
        }

        if (DisenableMovement != null) DisenableMovement(true);
        cooldownHitboxesController.Attack(angle); 
        yield return new WaitForSeconds(cooldownHitboxesController.hitbox.duration);


        canAttack = true; 
        yield break; 
    }
}
