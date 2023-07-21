using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitboxInfo2 {
    [Header ("Shape")]
    public Vector2 size; 
    public Vector2 offset; 
    public Vector2 local_position; 
    public HitboxShape shape; 

    public RigidbodyInfo rigidbodyInfo; 

    [Header("Timing")]
    public float duration; 
    [Range(0, 10)]
    public int repeat; 

    [Header("Movement")]
    public Vector2 move; 
    public bool isBody;

    [Header("Animation")]
    public List<Sprite> sprites;

    [Header("Damage")]
    public DamageInfoOld damageinfo; 

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

