using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.Colliders.Hitbox2d;

namespace Laserbean.AttackHitbox2D
{
[System.Serializable]
public class AttackHitboxInfo {
    [Header ("Shape")]

    public HitboxShapeData HitboxShapeInfo; 
    public RigidbodyInfo rigidbodyInfo; 
    public bool zeroRotation = false; 

    [Header("Timing")]
    public float duration; 

    public float lifetime {get => _after_duration + duration;}

    [SerializeField] float _after_duration; 

    
    [Range(0, 10)]
    public int repeat; 

    [Header("Movement")]
    public MovementInfo movementInfo; 

    [Header("Animation")]
    public List<Sprite> sprites;

    public GameObject prefab; 

    [Header("Damage")]
    public DamageInfo damageinfo; 

    [Header("Stain/Splash")] 
    public StainInfo stain_info; 

    [Min(1)]
    [Range(1, 100)]
    public int bullets = 1; 

}


[System.Serializable]
public struct MovementInfo {
    public Vector2 move; 
    public Vector2 bodymove; 
    public bool isBody;

}



[System.Serializable] 
public struct StainInfo {
    public StainType stainType; 
    public int value; 
}


}