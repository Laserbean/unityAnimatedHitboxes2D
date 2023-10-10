using System.Collections;
using System.Collections.Generic;
using Laserbean.Colliders;
using Laserbean.General;
using Laserbean.Hitbox2D;
using UnityEngine;



public static class HitboxCreator 
{

    public static GameObject CreateHitbox(HitboxShapeData hitboxShapeData, RigidbodyInfo rigidbodyInfo) {

        var gameObject = new GameObject("new hitbox"); 
        
        gameObject.AddComponent<Rigidbody2D>(); 
        gameObject.AddComponent<SpriteRenderer>(); 

        
        var hitboxthing = gameObject.AddComponent<HitboxControllerNew>(); 
        // hitboxthing.SetupCollider(hitboxShapeData, istrigger:true);



        if (gameObject.GetComponent<CustomTag>() == null) {
            gameObject.AddComponent<CustomTag>(); 
        }

        gameObject.GetComponent<CustomTag>().AddTag(Constants.TAG_HITBOX);
        gameObject.tag = Constants.TAG_HITBOX;


        CreateColliders(ref gameObject, hitboxShapeData, istrigger:true);
        SetupRigidbody(ref gameObject, rigidbodyInfo);


        return gameObject; 
    }

    public static void CreateColliders(ref GameObject gggg, HitboxShapeData  hitboxshape, bool istrigger) {

        switch(hitboxshape.shape) {
            case HitboxShape.Rectangle:
                var boxCollider2D = gggg.AddComponent<BoxCollider2D>();
                boxCollider2D.offset = hitboxshape.offset; 
                boxCollider2D.size = hitboxshape.size; 
                boxCollider2D.isTrigger = istrigger; 

                boxCollider2D.enabled = true; 
            break;
            case HitboxShape.Circle:
                var circleCollider2D = gggg.AddComponent<CircleCollider2D>();
                circleCollider2D.offset = hitboxshape.offset; 
                circleCollider2D.radius = hitboxshape.size[0]; 
                circleCollider2D.isTrigger = istrigger; 

                circleCollider2D.enabled = true; 
            break;
            case HitboxShape.Sector:
                var polygonCollider2D = gggg.AddComponent<PolygonCollider2D>();
                polygonCollider2D.GenerateSectorCollider(hitboxshape.size[1], 90f - hitboxshape.size[1]/2, hitboxshape.size[0], hitboxshape.size[0]/10, 4);
                polygonCollider2D.offset = hitboxshape.offset; 
                polygonCollider2D.isTrigger = istrigger; 
                polygonCollider2D.enabled = true; 
            break;
            default:
            Debug.LogError("Is this thing actually here"); 
            break; 
        
        }
    }

    public static void SetupRigidbody(ref GameObject go, RigidbodyInfo rigidbodyInfo) {
        var rgbd2d = go.GetComponent<Rigidbody2D>(); 
        rgbd2d.gravityScale   = rigidbodyInfo.gravity_scale; 
        rgbd2d.mass           = rigidbodyInfo.mass; 
        rgbd2d.drag           = rigidbodyInfo.linear_drag; 
        rgbd2d.freezeRotation = rigidbodyInfo.freeze_rotation; 
    }

}
