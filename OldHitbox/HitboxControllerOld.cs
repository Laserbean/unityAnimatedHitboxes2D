#define USING_LASERBEAN_CHUNKS_2D


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.General; 
using Laserbean.Colliders; 

using System.Linq; 

#if USING_LASERBEAN_CHUNKS_2D
using Laserbean.Chunks2d;
#endif

using Laserbean.General.OtherInterfaces;


using unityInventorySystem;
using Unity.Mathematics;

using Laserbean.Colliders.Hitbox2d;


namespace Laserbean.AttackHitbox2D
{
public class HitboxControllerOld : MonoBehaviour
{

    SpriteRenderer spriteRenderer; 

    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] CircleCollider2D circleCollider2D;
    [SerializeField] PolygonCollider2D polygonCollider2D;


    Rigidbody2D rgbd2d; 

    Transform parent; 

    AttackHitboxInfo hitbox_info;

    List<string> blacklist_tags_list = new List<string>();


    public void SetBlacklist(List<string> list) {
        blacklist_tags_list = list; 
    }

    public bool canAttack = true; 

    IDamageModify iDamageModify; 
    IOnHit iOnHit; 

    private void Start() {
        if (spriteRenderer == null) spriteRenderer = this.GetComponent<SpriteRenderer>(); 
        if (rgbd2d == null) rgbd2d = this.GetComponent<Rigidbody2D>(); 

    }


    public void Initialize() {

        iDamageModify = GetComponentInParent<IDamageModify>(); 
        iDamageModify ??= transform.parent.GetComponentInParent<IDamageModify>();
        iDamageModify ??= transform.parent.parent.GetComponentInParent<IDamageModify>();

        iOnHit = GetComponentInParent<IOnHit>(); 
        iOnHit ??= transform.parent.GetComponentInParent<IOnHit>(); 
        iOnHit ??= transform.parent.parent.GetComponentInParent<IOnHit>(); 

        rgbd2d ??= this.gameObject.AddComponent<Rigidbody2D>(); 

        spriteRenderer  ??=  this.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Entities";

        boxCollider2D ??= this.gameObject.AddComponent<BoxCollider2D>(); 
        boxCollider2D.enabled = false;


        circleCollider2D ??= this.gameObject.AddComponent<CircleCollider2D>(); 
        circleCollider2D.enabled = false;
    
        polygonCollider2D ??= this.gameObject.AddComponent<PolygonCollider2D>(); 
        polygonCollider2D.enabled = false;
        
    }

    void SetupCollider(AttackHitboxInfo hitbox) {
        boxCollider2D.enabled = false; 
        circleCollider2D.enabled = false; 
        polygonCollider2D.enabled = false; 

        boxCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 
        circleCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 
        polygonCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 

        var hitboxshape = hitbox.HitboxShapeInfo; 

        switch(hitboxshape.shape) {
            
            case HitboxShape.Rectangle:

                boxCollider2D.offset = hitboxshape.offset; 
                boxCollider2D.size = hitboxshape.size; 
                boxCollider2D.enabled = true; 
            break;
            case HitboxShape.Circle:

                circleCollider2D.offset = hitboxshape.offset; 
                circleCollider2D.radius = hitboxshape.size[0]; 
                circleCollider2D.enabled = true; 
            break;
            case HitboxShape.Sector:

                polygonCollider2D.GenerateSectorCollider(hitboxshape.size[1], 90f - hitboxshape.size[1]/2, hitboxshape.size[0], hitboxshape.size[0]/10, 4);
                polygonCollider2D.offset = hitboxshape.offset; 
                polygonCollider2D.enabled = true; 
            break;
            default:
            break; 
        
        }
    }

    public void SetupHitbox(AttackHitboxInfo _hitbox, Transform _parent = null) {

        if (spriteRenderer == null) spriteRenderer = this.GetComponent<SpriteRenderer>(); 
        if (rgbd2d == null) rgbd2d = this.GetComponent<Rigidbody2D>(); 

        hitbox_info = _hitbox; 

        SetupCollider(hitbox_info); 
        parent = _parent; 

        // // // if (_hitbox.isBody) {
        // // //     this.gameObject.transform.parent = _parent; 
        // // //     this.gameObject.transform.position = _parent.position + _hitbox.offset.ToVector3().Rotate(_parent.rotation.eulerAngles.z); 
        // // //     this.gameObject.transform.localRotation =Quaternion.identity; 
        // // // }

        rgbd2d.gravityScale   = _hitbox.rigidbodyInfo.gravity_scale; 
        rgbd2d.mass           = _hitbox.rigidbodyInfo.mass; 
        rgbd2d.drag           = _hitbox.rigidbodyInfo.linear_drag; 
        rgbd2d.freezeRotation = _hitbox.rigidbodyInfo.freeze_rotation; 
    }


    public void SetKinematic(bool aa) {
        rgbd2d.isKinematic = aa; 
    }


    #region Collider_damage
    void DoDamage() {        
        if(boxCollider2D.enabled) DoDamageCollider(boxCollider2D);
        if(circleCollider2D.enabled) DoDamageCollider(circleCollider2D);
        if(polygonCollider2D.enabled) DoDamageCollider(polygonCollider2D);
    }

    void DoDamageCollider(Collider2D triggerCollider) {    
        // Collider2D[] colliders = Physics2D.OverlapBoxAll(triggerCollider.bounds.center, triggerCollider.bounds.size, 0f);
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
            DamageOther(collider.gameObject, hitbox_info.damageinfo);
            number_hit += 1; 
        }

        iOnHit?.OnHit(number_hit); 

#if USING_LASERBEAN_CHUNKS_2D
        Damage damage_to_deal = hitbox_info.damageinfo.GetDamage(Vector2.one); 
        iDamageModify?.ModifyDamage(ref damage_to_deal); 
        EventManager.TriggerEvent(new OnHitboxAttack(triggerCollider, damage_to_deal.damage));
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

        // // // var idmginfo = other.GetComponent<IDamageInfoable>(); 
        // // // if (idmginfo != null) {
        // // //     idmginfo.DamageInfoed(dmginfo); 
        // // //     return; 
        // // // }

        other.GetComponent<IDamageableInt>()?.Damage(dmginfo.damage_ammount); 


    }

    //NOTE this should only run if the hitbox is a projectile. 
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.isTrigger) return; 

        CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
        if (ctag == null) {
            if (blacklist_tags_list.Contains(other.gameObject.tag)) return; 
            // other.gameObject.GetComponent<IDamageable>()?.Damage(hitbox_info.damageinfo.damage); 
            DamageOther(other.gameObject, hitbox_info.damageinfo);


            turnOffCollider();
            return;
        }

        if (ctag.HasTag(Constants.TAG_HITBOX)) return; 

        List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 

        if (othertags.Count > 0) { 
            turnOffCollider();
            return; 
        }

        // other.gameObject.GetComponent<IDamageable>()?.Damage(hitbox_info.damageinfo.damage); 
        DamageOther(other.gameObject, hitbox_info.damageinfo);

        turnOffCollider();
        
    }

    void OnTriggerEnter2D(Collider2D other) //TODO Add a condition to check if damage should be done on trigger enter. 
    {
        if (other.isTrigger) return; 
        if (hitbox_info.rigidbodyInfo.canPassWalls) return; 
        if (!hitbox_info.rigidbodyInfo.isTrigger) return; 
        if (hitbox_info.movementInfo.move.sqrMagnitude == 0f) return; 


        // CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
        // if (ctag == null) return;
        // List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
        // if (othertags.Count > 0) { 
        //     return; 
        // }


        // if(IsMovingIntoCollider(other, 0) && 
        
        if (other.gameObject.HasTag(Constants.TAG_STOPS_PROJECTILES)) {
            // turnOffCollider();
            rgbd2d.velocity = new Vector2(rgbd2d.velocity.x /10f         , rgbd2d.velocity.y/10f);
        }


    }

    void turnOffCollider() {
        boxCollider2D.enabled = false; 
        circleCollider2D.enabled = false; 
        polygonCollider2D.enabled = false; 

        spriteRenderer.enabled = false; 

        TrailRenderer trailRenderer = this.gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        this.transform.parent = parent; 
        this.transform.localPosition = Vector3.zero; 

        // this.gameObject.SetActive(false); 

        Invoke("setInactive", 0.1f); 


    }

    void setInactive() {
        this.gameObject.SetActive(false); 
    }




    void resetCollider() {
        TrailRenderer trailRenderer = this.gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }

        spriteRenderer.enabled = true; 

        switch(hitbox_info.HitboxShapeInfo.shape) {
            case HitboxShape.Rectangle: 
                boxCollider2D.enabled = true; 
            break;
            case HitboxShape.Circle:
                circleCollider2D.enabled = true; 
            break;
            case HitboxShape.Sector:
                polygonCollider2D.enabled = true; 
            break;
            default:
            break; 
        
        }
    }
    #endregion

    #region attacking_stuff


    public Coroutine Attack(float angle) {
        this.gameObject.SetActive(true); 

        return StartCoroutine(AttackCoroutine(angle)); 
    }

    IEnumerator AttackCoroutine(float angle) {
        resetCollider(); 

        StartMoving(angle); 


        yield return StartCoroutine(allAttack()); 
        
        
        turnOffCollider(); 

        // this.transform.localPosition = Vector3.zero; 
        if (parent == null) Destroy(this.gameObject); 
        this.transform.parent = parent; 
        this.transform.localPosition = Vector3.zero; 
    }

    IEnumerator allAttack() {
        if (hitbox_info == null) {
            Debug.LogError("hitbox info is null"); 
            yield break; 
        }

        List<Sprite> anim = hitbox_info.sprites; 
        int num = anim.Count; 

        for (int i = 0; i < hitbox_info.repeat + 1; i++) {
            // hasAttacked = false;
            if (num > 0) {
                StartCoroutine(spriteRenderer.DoAnimationTotalTime(hitbox_info.sprites, hitbox_info.lifetime));
            } 
            yield return new WaitForSeconds(hitbox_info.duration);            
            if (hitbox_info.rigidbodyInfo.isTrigger) {
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
        
        Vector3 move = hitbox_info.movementInfo.move.Rotate(angle); 
        // rgbd2d.isKinematic = false;

        // if (!hitbox_info.isBody) 
        rgbd2d.AddForce(move, ForceMode2D.Impulse);
    }




    private void OnDrawGizmos() {
        if(boxCollider2D.enabled) boxCollider2D.Draw(Color.green, this.transform);
        if(circleCollider2D.enabled) circleCollider2D.Draw(Color.green, this.transform);
        if(polygonCollider2D.enabled) polygonCollider2D.Draw(Color.green, this.transform);        
    }


}


}

