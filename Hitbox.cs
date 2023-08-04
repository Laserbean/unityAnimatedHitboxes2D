using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.General; 
using Laserbean.Colliders; 
namespace Laserbean.Hitbox2D
{
public class Hitbox : MonoBehaviour
{

    SpriteRenderer spriteRenderer; 

    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] CircleCollider2D circleCollider2D;
    [SerializeField] PolygonCollider2D polygonCollider2D;

    

    Rigidbody2D rgbd2d; 

    Transform parent; 

    HitboxInfo hitbox_info;

    List<string> blacklist_tags_list = new List<string>();
    // public void AddBlacklist(string tagg) {
    //     if (blacklist_tags_list.Contains(tagg)) return; 
    //     blacklist_tags_list.Add(tagg); 
    // }

    public void SetBlacklist(List<string> list) {
        blacklist_tags_list = list; 
    }

    public bool canAttack = true; 

    private void Start() {
        if (spriteRenderer == null) spriteRenderer = this.GetComponent<SpriteRenderer>(); 
        if (rgbd2d == null) rgbd2d = this.GetComponent<Rigidbody2D>(); 

    }



    public void awake() {
        if (rgbd2d == null)
        rgbd2d = this.gameObject.AddComponent<Rigidbody2D>(); 

        if (spriteRenderer == null) {
            spriteRenderer =  this.gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Entities";
        }

        if (boxCollider2D == null) {
            boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>(); 
            boxCollider2D.enabled = false;
        }


        if (circleCollider2D == null) {
            circleCollider2D = this.gameObject.AddComponent<CircleCollider2D>(); 
            circleCollider2D.enabled = false;
        }

        if (polygonCollider2D == null) {
            polygonCollider2D = this.gameObject.AddComponent<PolygonCollider2D>(); 
            polygonCollider2D.enabled = false;
        }


        // SetupHitbox(hitbox, parenttrans); 
        // rgbd2d.gravityScale = hitbox.rigidbodyInfo.gravity_scale; 
        // rgbd2d.mass = hitbox.rigidbodyInfo.mass; 
        // rgbd2d.drag = hitbox.rigidbodyInfo.linear_drag; 
        // rgbd2d.freezeRotation = hitbox.rigidbodyInfo.freeze_rotation; 
        
        // this.gameObject.SetActive(false); 

        turnOffCollider(); 

        this.gameObject.SetActive(false); 



    }

    void SetupCollider(HitboxInfo hitbox) {
        boxCollider2D.enabled = false; 
        circleCollider2D.enabled = false; 
        polygonCollider2D.enabled = false; 

        boxCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 
        circleCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 
        polygonCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger; 

        switch(hitbox.shape) {
            
            case HitboxShape.Rectangle:

                boxCollider2D.offset = hitbox.offset; 
                boxCollider2D.size = hitbox.size; 
                boxCollider2D.enabled = true; 
            break;
            case HitboxShape.Circle:

                circleCollider2D.offset = hitbox.offset; 
                circleCollider2D.radius = hitbox.size[0]; 
                circleCollider2D.enabled = true; 
            break;
            case HitboxShape.Sector:

                polygonCollider2D.GenerateSectorCollider(hitbox.size[1], 90f - hitbox.size[1]/2, hitbox.size[0], hitbox.size[0]/10, 4);
                polygonCollider2D.offset = hitbox.offset; 
                polygonCollider2D.enabled = true; 
            break;
            default:
            break; 
        
        }
    }

    public void SetupHitbox(HitboxInfo _hitbox, Transform _parent = null) {

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




    #region Collider_damage
    void DoDamage() {        
        if(boxCollider2D.enabled) DoDamageCollider(boxCollider2D);
        if(circleCollider2D.enabled) DoDamageCollider(circleCollider2D);
        if(polygonCollider2D.enabled) DoDamageCollider(polygonCollider2D);
    }

    void DoDamageCollider(Collider2D triggerCollider) {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(triggerCollider.bounds.center, triggerCollider.bounds.size, 0f);

        foreach (Collider2D collider in colliders) {
            if (!collider.isTrigger) {
                CustomTag ctag = collider.gameObject.GetComponent<CustomTag>(); 
                if (ctag != null) {
                    List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
                    if (othertags.Count > 0) { continue; }
                }

                if (blacklist_tags_list.Contains(collider.gameObject.tag)) continue; 

                collider.gameObject.GetComponent<IDamageable>()?.Damage(hitbox_info.damageinfo.damage); 
            }
        }

    }

    //NOTE this should only run if the hitbox is a projectile. 
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.isTrigger) return; 

        CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
        if (ctag == null) {
            turnOffCollider();
            return;
        }

        if (ctag.HasTag(Constants.TAG_HITBOX)) return; 

        List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 

        if (othertags.Count > 0) { 
            turnOffCollider();
            return; 
        }

        other.gameObject.GetComponent<IDamageable>()?.Damage(hitbox_info.damageinfo.damage); 
        turnOffCollider();
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return; 
        if (hitbox_info.rigidbodyInfo.canPassWalls) return; 
        if (!hitbox_info.rigidbodyInfo.isTrigger) return; 
        if (hitbox_info.move.sqrMagnitude == 0f) return; 


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

        switch(hitbox_info.shape) {
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
                StartCoroutine(this.gameObject.DoAnimation(spriteRenderer, hitbox_info.sprites, hitbox_info.lifetime));
            } 
            yield return new WaitForSeconds(hitbox_info.duration);            
            if (hitbox_info.rigidbodyInfo.isTrigger) {
                DoDamage(); 
            }
            yield return new WaitForSeconds(Mathf.Max(0, hitbox_info.lifetime - hitbox_info.duration));            

        }
    }



    #endregion


    void StartMoving(float angle) {
        if (rgbd2d == null) {
            rgbd2d = this.GetComponent<Rigidbody2D>();
        }
        
        Vector3 move = hitbox_info.move.Rotate(angle); 
        rgbd2d.isKinematic = false;
        rgbd2d.AddForce(move, ForceMode2D.Impulse);
    }




    private void OnDrawGizmos() {
        if(boxCollider2D.enabled) boxCollider2D.Draw(Color.green, this.transform);
        if(circleCollider2D.enabled) circleCollider2D.Draw(Color.green, this.transform);
        if(polygonCollider2D.enabled) polygonCollider2D.Draw(Color.green, this.transform);        
    }


}}

