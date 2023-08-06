using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laserbean.Hitbox2D
{
[System.Serializable]
public class HitboxInfo {
    [Header ("Shape")]
    public Vector2 size; 
    public Vector2 offset; 
    public Vector2 local_position; 
    public HitboxShape shape; 

    public RigidbodyInfo rigidbodyInfo; 

    public bool zeroRotation = false; 

    [Header("Timing")]
    public float duration; 

    public float lifetime {get => _after_duration + duration;}

    [SerializeField] float _after_duration; 

    
    [Range(0, 10)]
    public int repeat; 

    [Header("Movement")]
    public Vector2 move; 
    public Vector2 bodymove; 
    public bool isBody;

    [Header("Animation")]
    public List<Sprite> sprites;
    public GameObject prefab; 


    [Header("Damage")]
    public DamageInfo damageinfo; 

    [Min(1)]
    [Range(1, 100)]
    public int bullets = 1; 

}

[System.Serializable]
public class RigidbodyInfo {
    public bool isTrigger;
    public bool canPassWalls; //danny phantom phantom phantom
    public float mass; 
    public float linear_drag; 
    public float gravity_scale; 
    public bool freeze_rotation;
}

public enum HitboxShape {
    None,
    Rectangle,
    Circle, 
    Sector
}

}