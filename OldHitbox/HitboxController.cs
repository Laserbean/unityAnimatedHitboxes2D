using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.General;
using Laserbean.Colliders;

using Laserbean.General.OtherInterfaces;

using Laserbean.Colliders.Hitbox2d;

namespace Laserbean.AttackHitbox2D
{
public class HitboxController : MonoBehaviour
{

    AttackHitboxInfo hitbox_info; 
    SpriteRenderer spriteRenderer; 

    Transform parent = null; 

    bool hasAttacked = false; 

    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] CircleCollider2D circleCollider2D;
    [SerializeField] PolygonCollider2D polygonCollider2D;

    Rigidbody2D rgbd2d; 


    List<string> blacklist_tags_list = new List<string>();
    public void AddBlacklist(string tagg) {
        if (blacklist_tags_list.Contains(tagg)) return; 
        blacklist_tags_list.Add(tagg); 
    }


    public void SetupHitbox(AttackHitboxInfo _hitbox, Transform _parent) {

        hitbox_info = _hitbox; 

        SetupCollider(hitbox_info); 
        parent = _parent; 

        if (_hitbox.movementInfo.isBody) {
            this.gameObject.transform.parent = _parent; 
            this.gameObject.transform.position = _parent.position + _hitbox.HitboxShapeInfo.offset.ToVector3().Rotate(_parent.rotation.eulerAngles.z); 
            this.gameObject.transform.localRotation =Quaternion.identity; 
        }
    }

    void SetupCollider(AttackHitboxInfo hitbox) {
        boxCollider2D.enabled = false; 
        circleCollider2D.enabled = false; 
        polygonCollider2D.enabled = false; 

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




    public Coroutine Attack(float angle, Vector3 pos) {
        this.gameObject.SetActive(true); 
        return StartCoroutine(AttackCoroutine(angle, pos)); 
    }

    IEnumerator AttackCoroutine(float angle, Vector3 pos) {

        ResetPosition(angle, pos); 
        StartMoving(angle); 
        yield return StartCoroutine(allAttack()); 
        
        
        this.gameObject.SetActive(false); 
        this.transform.parent = parent; 

        this.transform.localPosition = Vector3.zero; 
        resetCollider(); 
        if (parent == null) Destroy(this.gameObject); 
    }

    void turnOffCollider() {
        boxCollider2D.enabled = false; 
        circleCollider2D.enabled = false; 
        polygonCollider2D.enabled = false; 

        spriteRenderer.enabled = false; 
    }

    void resetCollider() {
        spriteRenderer.enabled = true; 

        var hitboxshape = hitbox_info.HitboxShapeInfo; 
        switch(hitboxshape.shape) {
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


    void SetPosition(float angle, Vector3 position) {
        this.transform.parent = null; 

        this.gameObject.transform.position = position + hitbox_info.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 

        angle = hitbox_info.zeroRotation ? 0 : angle; 
        this.transform.rotation = Quaternion.Euler(0,0,angle); 

    }

    
    void ResetPosition(float angle, Vector3 position) {
        angle = hitbox_info.zeroRotation ? 0 : angle; 

        if (hitbox_info.movementInfo.isBody) {
            this.gameObject.transform.position = position + hitbox_info.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            this.transform.parent = parent; 
            // this.gameObject.transform.localPosition = hitbox_info.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {
            this.transform.parent = null; 
            this.gameObject.transform.position = position + hitbox_info.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle); 
        }
    }


    void ResetPosition(float angle) {
        angle = hitbox_info.zeroRotation ? 0 : angle; 

        if (hitbox_info.movementInfo.isBody) {
            this.transform.parent = parent; 
            this.gameObject.transform.localPosition = hitbox_info.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {
            this.transform.parent = null; 
            this.gameObject.transform.position =parent.transform.position + hitbox_info.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle); 
        }
    }

    IEnumerator allAttack() {

        List<Sprite> anim = hitbox_info.sprites; 
        int num = anim.Count; 

        for (int i = 0; i < hitbox_info.repeat + 1; i++) {
            // hasAttacked = false;
            if (num > 0) {
                yield return StartCoroutine(DoAnimation(hitbox_info.sprites, hitbox_info.duration));
            } else {
                yield return new WaitForSeconds(hitbox_info.duration); 
            }
    
            if (hitbox_info.rigidbodyInfo.isTrigger) {
                DoDamage(); 
            }
        }
    }


    IEnumerator DoAnimation(List<Sprite> sprites, float total_time) {
        int nnnn = sprites.Count;
        for (int j = 0; j < nnnn; j++) {
            spriteRenderer.sprite = sprites[j];
            yield return new WaitForSeconds(total_time/nnnn); 
        }
    }

    void StartMoving(float angle) {
        if (rgbd2d == null) {
            rgbd2d = this.GetComponent<Rigidbody2D>();
        }
        Vector3 move = hitbox_info.movementInfo.move.Rotate(angle); 
        if (hitbox_info.movementInfo.isBody) {
            rgbd2d.isKinematic = true;

            if(parent.GetComponent<Rigidbody2D>() != null)
                parent.GetComponent<Rigidbody2D>()?.AddForce(move, ForceMode2D.Impulse); 
            
            forceee = Vector2.zero; 
        } else {
            rgbd2d.isKinematic = false;
            forceee = move; 
            rgbd2d.AddForce(move, ForceMode2D.Impulse);
        }
    }

    Vector2 forceee = Vector2.zero; 

    private void FixedUpdate() {
        if (rgbd2d == null) {
            rgbd2d = this.GetComponent<Rigidbody2D>();
        }
        // rgbd2d.AddForce(forceee); 
    }


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
                    if (ctag == null) continue;

                    List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
                    if (othertags.Count > 0) { continue; }
                collider.gameObject.GetComponent<IDamageableInt>()?.Damage(hitbox_info.damageinfo.damage_ammount); 
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.collider.isTrigger) {
            CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
            if (ctag == null) return;

            List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 

            if (othertags.Count > 0) { 
                turnOffCollider();
                return; 
            }

            other.gameObject.GetComponent<IDamageableInt>()?.Damage(hitbox_info.damageinfo.damage_ammount); 
            turnOffCollider();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitbox_info.rigidbodyInfo.canPassWalls) return; 
        if (!hitbox_info.rigidbodyInfo.isTrigger) return; 
        if (hitbox_info.movementInfo.move.sqrMagnitude == 0f) return; 


        // CustomTag ctag = other.gameObject.GetComponent<CustomTag>(); 
        // if (ctag == null) return;
        // List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
        // if (othertags.Count > 0) { 
        //     return; 
        // }

        if(IsMovingIntoCollider(other, 0) && other.gameObject.HasTag(Constants.TAG_STOPS_PROJECTILES)) {
            // turnOffCollider();
            rgbd2d.velocity = new Vector2(rgbd2d.velocity.x /10f         , rgbd2d.velocity.y/10f);
        }


    }


    private bool IsMovingIntoCollider(Collider2D otherCollider, float tolerance = 0f)
    {
        // Compare the direction of the projectile with the normal of the other collider
        Vector2 direction = GetComponent<Rigidbody2D>().velocity.normalized;
        Vector2 normal = otherCollider.ClosestPoint(transform.position) - transform.position.ToVector2();
        return Vector2.Dot(direction, normal) > tolerance;
    }

    Collider2D coltodraw; 
    private void OnDrawGizmos() {
    if(boxCollider2D.enabled) boxCollider2D.Draw(Color.green, this.transform);
    if(circleCollider2D.enabled) circleCollider2D.Draw(Color.green, this.transform);
    if(polygonCollider2D.enabled) polygonCollider2D.Draw(Color.green, this.transform);

        
    }


    public static GameObject CreateHitbox(Transform parenttrans, AttackHitboxInfo hitbox, string nam = "hitbox") {
        GameObject hitboxobject = new GameObject(nam); 

        // hitboxobject.transform.SetParent(parenttrans); 

        hitboxobject.transform.parent = parenttrans; 

        HitboxController hbctrl = hitboxobject.AddComponent<HitboxController>();
        hbctrl.spriteRenderer =  hitboxobject.gameObject.AddComponent<SpriteRenderer>();
        hbctrl.spriteRenderer.sortingLayerName = "Entities";


        hbctrl.boxCollider2D = hitboxobject.AddComponent<BoxCollider2D>(); 
        hbctrl.boxCollider2D.enabled = false;
        hbctrl.boxCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger;  
        hbctrl.circleCollider2D = hitboxobject.AddComponent<CircleCollider2D>(); 
        hbctrl.circleCollider2D.enabled = false;
        hbctrl.circleCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger;  
        hbctrl.polygonCollider2D = hitboxobject.AddComponent<PolygonCollider2D>(); 
        hbctrl.polygonCollider2D.enabled = false;
        hbctrl.polygonCollider2D.isTrigger = hitbox.rigidbodyInfo.isTrigger;   


        hbctrl.SetupHitbox(hitbox, parenttrans); 

        hbctrl.rgbd2d = hitboxobject.AddComponent<Rigidbody2D>(); 

        hbctrl.rgbd2d.gravityScale = hitbox.rigidbodyInfo.gravity_scale; 
        hbctrl.rgbd2d.mass = hitbox.rigidbodyInfo.mass; 
        hbctrl.rgbd2d.drag = hitbox.rigidbodyInfo.linear_drag; 
        hbctrl.rgbd2d.freezeRotation = hitbox.rigidbodyInfo.freeze_rotation; 

        hitboxobject.SetActive(false); 

        return hitboxobject;
    }

}
}
