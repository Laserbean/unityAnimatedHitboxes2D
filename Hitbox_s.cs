using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hitbox_s : MonoBehaviour
{
    // Start is called before the first frame update

    float duration;
    int repeat; 

    bool isEnemy; 

    List<string> target_tags_list = new List<string>(); 

    DamageInfoOld damageInfo; 

    public void init(List<string> _targets, DamageInfoOld damageinfo, float _duration, int _repeat = 1) {
        damageInfo = damageinfo;
        duration = _duration;
        repeat = _repeat;
        target_tags_list = _targets; 
    }

    public void init(string _target, DamageInfoOld damageinfo, float _duration, int _repeat = 1) {
        damageInfo = damageinfo;
        duration = _duration;
        repeat = _repeat;
        target_tags_list.Add(_target); 
    }

    public void init(string _target, AttackInfoOld attackInfo, Transform _parent) {
        damageInfo = attackInfo.damageinfo; 
        duration = attackInfo.duration;
        repeat = attackInfo.repeat; 
        target_tags_list.Add(_target); 

        SetupHitboxes(attackInfo, _parent);
    }

    public void SetupHitboxes(AttackInfoOld attack, Transform _parent) {
        SpriteRenderer spren = this.gameObject.AddComponent<SpriteRenderer>(); 
        spren.sprite = attack.sprite; 


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
            col.GenerateSectorCollider(attack.hitbox.size[0], 90f - attack.hitbox.size[0]/2, attack.hitbox.size[1], attack.hitbox.size[0]/10, 4);
            col.offset = attack.hitbox.offset; 
            col.isTrigger = true; 
        }

        isBody = attack.isBody; 
        parent = _parent; 
        offset = attack.hitbox.offset; 

        if (attack.isBody) {
            this.gameObject.transform.parent = _parent; 
            this.gameObject.transform.position = _parent.position + attack.hitbox.offset.ToVector3().Rotate(_parent.rotation.eulerAngles.z); 
            this.gameObject.transform.localRotation =Quaternion.identity; 
        }
    }

    bool isBody = false;
    Transform parent = null; 
    Vector2 offset; 

    bool hasAttacked = false; 

    public void Attack() {
        if (!isBody) {
            this.gameObject.transform.position = parent.position + offset.ToVector3().Rotate(parent.rotation.eulerAngles.z); 
            this.transform.rotation = parent.rotation; 
        }


        // Debug.Log("hitbox s attack");
        this.gameObject.SetActive(true); 
        StartCoroutine(Attack_and_Disappear()); 
    }


    IEnumerator Attack_and_Disappear() {
        for (int i = 0; i < repeat; i++) {
            hasAttacked = false;

            yield return new WaitForSeconds(duration); 
        }


        this.gameObject.SetActive(false); 
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.isTrigger) {
            return; 
        }

        CustomTag ctag = other.GetComponent<CustomTag>(); 
        if (ctag == null) {
            return;
        }
        

        List<string> othertags = ctag.ContainedTags(target_tags_list); 
        
        if (othertags.Count > 0) {
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
                // if (damageInfo.allStatusEffects != null) {
                // }

                hasAttacked = true; 

            }
        }

    }


    // // private void Update() {
    // //     Debug.DrawLine(col.GetShapes())
    // // }
}
