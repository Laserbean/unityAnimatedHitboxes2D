using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_v2 : MonoBehaviour
{

    AttackInfoOld attackinfo; 
    SpriteRenderer spriteRenderer; 


    List<string> blacklist_tags_list = new List<string>();

    
    private void Awake() {
        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>(); 
    }

    public void AddBlacklist(string tagg) {
        blacklist_tags_list.Add(tagg); 
    }


    public void SetupHitboxes(AttackInfoOld attack, Transform _parent) {
        if (attack.isBody) {
            this.transform.parent = _parent; 
        }

        this.gameObject.SetActive(false); 

        // if(attackertag != null)
        // blacklist_tags_list.Add(attackertag); 
                
        attackinfo = attack; 
        
        spriteRenderer.sortingLayerName = "Entities";
        spriteRenderer.sprite = attack.sprite; 

        //NOTE Switch doesn't work for this. 
        if (attack.hitbox.shape == HitboxShape.Rectangle){
            BoxCollider2D col = this.gameObject.AddComponent<BoxCollider2D>(); 
            col.offset = attack.hitbox.offset; 
            col.size = attack.hitbox.size; 
            col.isTrigger = true; 

        } else if (attack.hitbox.shape == HitboxShape.Circle) {
            CircleCollider2D col = this.gameObject.AddComponent<CircleCollider2D>(); 
            col.offset = attack.hitbox.offset; 
            col.radius = attack.hitbox.size[0]; 
            col.isTrigger = true; 
        } else if (attack.hitbox.shape == HitboxShape.Sector) {
            PolygonCollider2D col = this.gameObject.AddComponent<PolygonCollider2D>();
            col.GenerateSectorCollider(attack.hitbox.size[1], 90f - attack.hitbox.size[1]/2, attack.hitbox.size[0], attack.hitbox.size[0]/10, 4);

            col.offset = attack.hitbox.offset; 
            col.isTrigger = true; 
        }
        
        parent = _parent; 

        if (attack.isBody) {
            this.gameObject.transform.parent = _parent; 
            this.gameObject.transform.position = _parent.position + attack.hitbox.offset.ToVector3().Rotate(_parent.rotation.eulerAngles.z); 
            this.gameObject.transform.localRotation =Quaternion.identity; 
        }
    }

    Transform parent = null; 

    bool hasAttacked = false; 


    public void Attack(float angle) {
        this.gameObject.SetActive(true); 

        StartCoroutine(AttackCoroutine(angle)); 
    }

    IEnumerator AttackCoroutine(float angle) {

        SetPosition(angle); 
        yield return StartCoroutine(DoAnimation(attackinfo.hitbox.pre_sprites, attackinfo.prep_time));

        MoveDoer(angle); 
        yield return StartCoroutine(Attack_and_Disappear()); 

        this.gameObject.SetActive(false); 
    }

    void SetPosition(float angle) {
        if (attackinfo.isBody) {
            this.transform.parent = parent; 
            this.gameObject.transform.localPosition = attackinfo.hitbox.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {
            this.transform.parent = null; 
            this.gameObject.transform.position =parent.transform.position + attackinfo.hitbox.local_position.ToVector3().Rotate(angle); 
            this.transform.rotation = Quaternion.Euler(0,0,angle); 
        }
    }

    IEnumerator DoAnimation(List<Sprite> sprites, float total_time) {
        int nnnn = sprites.Count;
        for (int j = 0; j < nnnn; j++) {
            spriteRenderer.sprite = sprites[j];
            yield return new WaitForSeconds(total_time/nnnn); 
        }
    }

    void MoveDoer(float angle) {
        Vector3 move = attackinfo.move.Rotate(angle); 
        if (attackinfo.isBody) {
            this.transform.GetComponent<Rigidbody2D>().isKinematic = true;
            this.transform.parent.GetComponent<Rigidbody2D>().AddForce(move, ForceMode2D.Impulse); 
            forceee = Vector2.zero; 
        } else {
            this.transform.GetComponent<Rigidbody2D>().isKinematic = false;
            forceee = move; 
        }
    }

    Vector2 forceee = Vector2.zero; 
    Rigidbody2D rgbd2d; 


    IEnumerator Attack_and_Disappear() {

        List<Sprite> anim = attackinfo.hitbox.sprites; 
        int num = anim.Count; 

        for (int i = 0; i < attackinfo.repeat; i++) {
            // hasAttacked = false;
            if (num > 0) {
                 yield return StartCoroutine(DoAnimation(attackinfo.hitbox.sprites, attackinfo.duration));
            } else {
                yield return new WaitForSeconds(attackinfo.duration); 
            }
    
            DoDamage(); 
        }
    }

    void DoDamage() {

        Collider2D triggerCollider = this.GetComponent<Collider2D>(); 
        Collider2D[] colliders = Physics2D.OverlapBoxAll(triggerCollider.bounds.center, triggerCollider.bounds.size, 0f);

        foreach (Collider2D collider in colliders) {
            if (!collider.isTrigger) {
                    CustomTag ctag = collider.gameObject.GetComponent<CustomTag>(); 
                    if (ctag == null) continue;

                    List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
                    if (othertags.Count > 0) { continue; }
                collider.gameObject.GetComponent<IDamageable>()?.Damage(attackinfo.damageinfo.damage); 
            }
        }

    }


    Collider2D coltodraw; 
    private void OnDrawGizmos() {
        if (coltodraw == null) coltodraw = this.GetComponent<Collider2D>(); 

        coltodraw.Draw(Color.green, this.transform); 
        
    }


    private void FixedUpdate() {
        if (rgbd2d == null) {
            rgbd2d = this.GetComponent<Rigidbody2D>();
        }
        rgbd2d.AddForce(forceee); 
    }


    // private void OnTriggerExit2D(Collider2D other) {
    //     if(other.isTrigger) return; 

    //     if(other.gameObject.HasTag()) {

    //     }
    // }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.isTrigger) return; 
        CustomTag ctag = other.GetComponent<CustomTag>(); 
        if (ctag == null) return;
        

        List<string> othertags = ctag.ContainedTags(blacklist_tags_list); 
        if (othertags.Count > 0) { return; }
        
        DamageInfoOld damageInfo = attackinfo.damageinfo; 
            if (!hasAttacked) {
                var inter = other.GetComponentInParent<IKnockbackable>();
                inter?.Knockback(damageInfo.knockback * Vector2.up.Rotate(this.transform.rotation.eulerAngles.z));
                
                
                var inter2 = other.GetComponentInParent<IDamageable>();
                inter2?.Damage(damageInfo.damage);
                


                var inter3 = other.GetComponentInParent<IStunnable>();
                inter3?.Stun(damageInfo.stun); //TODO


                foreach(var statusEffect in damageInfo.allStatusEffects) {
                    other.GetComponentInParent<IStatusAffect>()?.AddStatusEffect(statusEffect.statusEffect, statusEffect.duration);

                }

                hasAttacked = true; 

            }
        

    }


    public static GameObject CreateHitbox(Transform parenttrans, AttackInfoOld attack) {
        GameObject hitboxobject = new GameObject("hitbox"); 

        hitboxobject.AddComponent<Hitbox_v2>();
        hitboxobject.GetComponent<Hitbox_v2>().SetupHitboxes(attack, parenttrans); 

        Rigidbody2D fish = hitboxobject.AddComponent<Rigidbody2D>(); 
        fish.gravityScale = 0; 

        return hitboxobject;
    }
}

