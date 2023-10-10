#define USING_LASERBEAN_CHUNKS_2D

using System.Collections;
using System.Collections.Generic;

using Laserbean.General;
using Laserbean.Colliders; 

using Laserbean.Hitbox2D;
using UnityEngine;
using unityInventorySystem;

#if USING_LASERBEAN_CHUNKS_2D
using Laserbean.Chunks2d;
#endif


public class HitboxAttackComponent : MonoBehaviour
{
    SpriteRenderer spriteRenderer; 

    Rigidbody2D rgbd2d; 

    Transform parent; 

    

    DamageInfo damageinfo; 
    RigidbodyInfo rgbdinfo; 
    List<Sprite> sprites; 
    MovementInfo movementInfo; 

    AttackHitboxInfo hitbox_info; 

    List<string> blacklist_tags_list = new List<string>();

    HitboxControllerNew hitboxControllerNew; 

    private void Awake() {
        hitboxControllerNew ??= GetComponent<HitboxControllerNew>(); 
        spriteRenderer ??= GetComponent<SpriteRenderer>(); 
        rgbd2d ??= GetComponent<Rigidbody2D>(); 
    }


    public void SetUp(AttackHitboxInfo _hitbox_info) {
        damageinfo = _hitbox_info.damageinfo; 
        rgbdinfo = _hitbox_info.rigidbodyInfo; 
        sprites = _hitbox_info.sprites; 

        movementInfo = _hitbox_info.movementInfo; 
        hitbox_info = _hitbox_info;
    }


    public void SetBlacklist(List<string> list) {
        blacklist_tags_list = list; 
    }

    public bool canAttack = true; 

    IDamageModify iDamageModify; 
    IOnHit iOnHit; 

    public void SetKinematic(bool aa) {
        rgbd2d.isKinematic = aa; 
    }

    #region Collider_damage
    void DoDamage() {        
        DoDamageCollider(GetComponent<HitboxControllerNew>().GetCurrentCollider());
    }

    void DoDamageCollider(Collider2D triggerCollider) {    
        List<Collider2D> colliders = new ();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(triggerCollider, filter, colliders);

        int number_hit = 0; 

        foreach (Collider2D collider in colliders) {
            if (collider.isTrigger) continue; //NOTE Not sure if i want t

            CustomTag ctag = collider.gameObject.GetComponent<CustomTag>(); 
            if (ctag != null) {
                List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
                if (othertags.Count > 0) { continue; }
            }

            if (blacklist_tags_list.Contains(collider.gameObject.tag)) continue; 
            DamageOther(collider.gameObject, damageinfo);
            number_hit += 1; 
        }

        iOnHit?.OnHit(number_hit); 

#if USING_LASERBEAN_CHUNKS_2D
        Damage damage_to_deal = damageinfo.GetDamage(Vector2.one); 
        iDamageModify?.ModifyDamage(ref damage_to_deal); 
        EventManager.TriggerEvent(new OnHitboxAttack(triggerCollider, damage_to_deal));
#endif

    }

    void DamageOther(GameObject other, DamageInfo dmginfo) {
        Vector3 knocback_dir = (other.transform.position - transform.position).normalized; 
        Damage damage_to_deal = dmginfo.GetDamage(knocback_dir); 
        iDamageModify?.ModifyDamage(ref damage_to_deal); 

        other.GetComponent<IDamageable2>()?.Damage(damage_to_deal); 

        var statusaffectable =  other.GetComponent<IStatusAffect>();
        if (statusaffectable != null) {
            foreach(var statuseffectdur in damage_to_deal.allStatusEffects) {
                statusaffectable.AddStatusEffect(statuseffectdur.statusEffect, statuseffectdur.duration);
            }
        }

        other.GetComponent<IDamageable>()?.Damage(dmginfo.damage_ammount); 


    }

    //NOTE this should only run if the hitbox is a projectile. 
    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.isTrigger) return; 

        CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
        if (ctag == null) {
            if (blacklist_tags_list.Contains(other.gameObject.tag)) return; 
            // other.gameObject.GetComponent<IDamageable>()?.Damage(damageinfo.damage); 
            DamageOther(other.gameObject, damageinfo);


            TurnOffCollider();
            return;
        }

        if (ctag.HasTag(Constants.TAG_HITBOX)) return; 

        List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 

        if (othertags.Count > 0) { 
            TurnOffCollider();
            return; 
        }

        // other.gameObject.GetComponent<IDamageable>()?.Damage(damageinfo.damage); 
        DamageOther(other.gameObject, damageinfo);

        TurnOffCollider();
        
    }

    void OnTriggerEnter2D(Collider2D other) //TODO Add a condition to check if damage should be done on trigger enter. 
    {
        if (other.isTrigger) return; 
        if (rgbdinfo.canPassWalls) return; 
        if (!rgbdinfo.isTrigger) return; 
        if (movementInfo.move.sqrMagnitude == 0f) return; 

        
        if (other.gameObject.HasTag(Constants.TAG_STOPS_PROJECTILES)) {
            // TurnOffCollider();
            rgbd2d.velocity = new Vector2(rgbd2d.velocity.x /10f         , rgbd2d.velocity.y/10f);
        }

    }

    void TurnOffCollider() {
        hitboxControllerNew.DisableColliders(); 

        spriteRenderer.enabled = false; 

        TrailRenderer trailRenderer = gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        transform.parent = parent; 
        transform.localPosition = Vector3.zero; 

        Invoke("setInactive", 0.1f); 
    }

    void setInactive() {
        gameObject.SetActive(false); 
    }

    void ResetCollider() {
        TrailRenderer trailRenderer = gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer != null) {
            trailRenderer.enabled = true;
        }

        spriteRenderer.enabled = true; 
        hitboxControllerNew.EnableCollider();
    }
    #endregion

    #region attacking_stuff


    public Coroutine Attack(float angle) {
        gameObject.SetActive(true); 

        return StartCoroutine(AttackCoroutine(angle)); 
    }

    IEnumerator AttackCoroutine(float angle) {
        ResetCollider(); 
        StartMoving(angle); 
        yield return StartCoroutine(AttackCoroutine()); 
        TurnOffCollider(); 

        // // if (parent == null) Destroy(gameObject); 
        
        transform.parent = parent; 
        transform.localPosition = Vector3.zero; 
    }

    IEnumerator AttackCoroutine() {

        List<Sprite> anim = sprites; 
        int num = anim.Count; 

        for (int i = 0; i < hitbox_info.repeat + 1; i++) {
            // hasAttacked = false;
            if (num > 0) {
                StartCoroutine(spriteRenderer.DoAnimationTotalTime(sprites, hitbox_info.lifetime));
            } 
            yield return new WaitForSeconds(hitbox_info.duration);            
            if (rgbdinfo.isTrigger) {
                DoDamage(); 
            } 
            yield return new WaitForSeconds(Mathf.Max(0, hitbox_info.lifetime));            

        }
    }



    #endregion


    void StartMoving(float angle) {
        if (rgbd2d == null) {
            rgbd2d = this.GetComponent<Rigidbody2D>();
        }
        
        Vector3 move = movementInfo.move.Rotate(angle); 
        // rgbd2d.isKinematic = false;

        // if (!hitbox_info.isBody) 
        rgbd2d.AddForce(move, ForceMode2D.Impulse);
    }

}
