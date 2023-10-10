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

using unityInventorySystem; 

namespace Laserbean.Hitbox2D
{
public class HitboxControllerNew : MonoBehaviour
{
    Collider2D collider_2D; 
    Rigidbody2D rgbd2d; 
    Transform parent; 

    HitboxShapeData hitboxinfo;

    public bool canAttack = true; 

    private void Start() {
        rgbd2d ??= this.GetComponent<Rigidbody2D>(); 
        collider_2D = this.GetComponent<Collider2D>(); 
    }

    private void OnEnable() {
        collider_2D = this.GetComponent<Collider2D>(); 
    }

    
    public void EnableCollider() {
        collider_2D.enabled = true; 
    }

    public void DisableColliders() {
        collider_2D.enabled = false; 
    }

    public Collider2D GetCurrentCollider() {
        return collider_2D; 
    }

    public void SetKinematic(bool aa) {
        rgbd2d ??= this.GetComponent<Rigidbody2D>(); 
        rgbd2d.isKinematic = aa; 
    }


    private void OnDrawGizmos() {
        if (collider_2D is BoxCollider2D boxCollider2D) boxCollider2D.Draw(Color.green, this.transform);
        if(collider_2D is CircleCollider2D circleCollider2D) circleCollider2D.Draw(Color.green, this.transform);
        if(collider_2D is PolygonCollider2D polygonCollider2D) polygonCollider2D.Draw(Color.green, this.transform);        
    }


}

}

