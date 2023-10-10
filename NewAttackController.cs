#define USING_ANIMATOR


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

    #if USING_ANIMATOR
    Animator animator; 

    const string attackParam = "isAttacking"; 
    bool hasParam = false; 
    #endif

    private void Awake() {
        #if USING_ANIMATOR
        animator = transform.parent.GetComponent<Animator>(); 
        if (animator == null )return; 
        hasParam = animator.ContainsParam(attackParam); 
        #endif
    }

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

        prepHGO = CreateHitboxGameobject("prep_hitbox");
        SetupHitboxController(ref prepHGO, ref prepHitboxesController, attackInfoObject.attack.prep_hitbox);

        attackHGOs.Clear(); 
        attackHitboxesControllers.Clear(); 
        int i = 0; 
        foreach(AttackHitboxInfo hitbox in attackInfoObject.attack.hitboxes) {
            var go = CreateHitboxGameobject("main_hitbox " + i++);   
            HitboxesController hitboxcont = null; 
            SetupHitboxController(ref go, ref hitboxcont, hitbox);

            attackHGOs.Add(go); 
            attackHitboxesControllers.Add(hitboxcont); 
        }

        cooldownHGO = CreateHitboxGameobject("cooldown"); 
        SetupHitboxController(ref cooldownHGO, ref cooldownHitboxesController, attackInfoObject.attack.after_hitbox);

        canAttack = true;
        UpdateBlacklist();
    }

    GameObject CreateHitboxGameobject(string namme) {
        prepHGO = new GameObject(namme); 
        prepHGO.transform.SetParent(transform); 
        prepHGO.transform.localPosition = Vector3.zero; 
        return prepHGO; 
    }

    void SetupHitboxController(ref GameObject cur_gameobject, ref HitboxesController hitboxesController, AttackHitboxInfo hitboxinfo) {
        hitboxesController = cur_gameobject.AddComponent<HitboxesController>(); 
        hitboxesController.SetHitbox(hitboxinfo); 
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

    public void StartAttack(float angle) {
        if(!GameManager.Instance.IsRunning) return; 
        if (!canAttack) return; 

        if (attackInfoObject == null) return; 

        StartCoroutine(Attackkk(angle)); 
    }

    IEnumerator Attackkk(float angle) {
        canAttack = false; 

        if (attackInfoObject.attack.lock_movement_while_attack) {
                DisenableMovement?.Invoke(false);
            }

        if (attackInfoObject.attack.lock_rotation_while_attack) {
                DisenableRotation?.Invoke(false);
            }

        Vector3 curpos = this.transform.position; 

        // yield return prepHitboxesController.Attack(angle, curpos);
        //TODO maybe add the pos here

        SetAttackAnimatorState(true); 
        prepHitboxesController.Attack(angle);
        if (prepHitboxesController.HitboxInfo.movementInfo.isBody && DoMovement != null) DoMovement(prepHitboxesController.HitboxInfo.movementInfo.bodymove.Rotate(angle));
        yield return new WaitForSeconds(prepHitboxesController.HitboxInfo.duration);


        
        foreach(var thing in attackHitboxesControllers) {
            thing.Attack(angle, attackInfoObject.attack.max_angle_error); 
            if (thing.HitboxInfo.movementInfo.isBody && DoMovement != null) DoMovement(thing.HitboxInfo.movementInfo.bodymove.Rotate(angle));

            yield return new WaitForSeconds(thing.HitboxInfo.lifetime);
            // yield return new WaitForSeconds(attackInfoObject.attack.attackDelay);
        }

        DisenableMovement?.Invoke(true);
        DisenableRotation?.Invoke(true);

        cooldownHitboxesController.Attack(angle); 

        SetAttackAnimatorState(false); 

        if (cooldownHitboxesController.HitboxInfo.movementInfo.isBody && DoMovement != null) DoMovement(cooldownHitboxesController.HitboxInfo.movementInfo.bodymove.Rotate(angle));
        yield return new WaitForSeconds(cooldownHitboxesController.HitboxInfo.duration);


        canAttack = true; 
        yield break; 
    }

    void SetAttackAnimatorState(bool param) {
        #if USING_ANIMATOR
        if (!hasParam) return; 
        animator?.SetBool(attackParam, param);
        #endif
    }
}
}