using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



[CreateAssetMenu(fileName = "EnemyInfoOld", menuName = "Enemies/EnemyInfoOld")]
public class EnemyInfoObjectOld : ScriptableObject
{
    public string Name; 
    public int id; 
    public GameObject prefab; 

    public EnemyInfoOld info;


    public List<AttackInfoObjectOld> attack_object_list; 



    private void OnValidate() {
        Name = this.name; 
        info.Name = Name; 

        List<AttackInfoOld> attackinfolist = new List<AttackInfoOld>(); 

        foreach(var attackinfoobject in attack_object_list) {
            attackinfolist.Add(attackinfoobject.attack); 
        }
        info.attack_list = attackinfolist;
    }

}


[System.Serializable]
public class EnemyInfoOld {
    public string Name;
    public int hp;

    public float mass;
    public float linear_drag;
    public float angular_drag; 

    public List<AttackInfoOld> attack_list;

}





[System.Serializable]
public class AttackInfoOld {
    public HitboxInfoOld hitbox; 
    public Sprite sprite; 

    public Vector2 move; 
    public bool isBody = false; 
    // public bool isBody; 

    [Header("Aim")]
    public float angle;
    [Tooltip("[Min, Max, Centre]")]
    public Vector3 range; //min, max, center;
    
    [Header("Timing")]
    public float prep_time = 0f;
    public float duration = 0f;
    public float cooldown_time = 0f;
    public float reload_time = 0f;

    public int repeat = 1; 
    // public int repeat; //For a double attack????

    [Header("Damage")]
    public DamageInfoOld damageinfo; 




}

[System.Serializable]
public struct HitboxInfoOld {
    public Vector2 size; 
    public Vector2 offset; 
    public HitboxShape shape; 

    public List<Sprite> sprites;
    public List<Sprite> pre_sprites;
    public List<Sprite> after_sprites;
    public Vector2 local_position; 

    // public HitboxInfo(Vector2 _size, Vector2 _offset, HitboxShape shape) {

    // }
}



[System.Serializable]
public class DamageInfoOld {
    [System.Serializable]
    public class StatusEffectDuration {
        public StatusEffectObject statusEffect; 
        public float duration;           
    }


    public int damage; 
    public float knockback; 
    public float stun; 
    public float critical; 


    public List<StatusEffectDuration> allStatusEffects = new List<StatusEffectDuration>(); 


    public DamageInfoOld(int dam, float knock, float stu, float crit) {
        critical = crit; 
        damage = dam; 
        knockback = knock;
        stun = stu;
        // debuff = new StatusEffect{type = DebuffType.None, duration = 0f, rate = 0f}; 

    }

    public DamageInfoOld(int dam, float knock, float stu, float crit, StatusEffectObject statusObject) {
        critical = crit; 

        damage = dam; 
        knockback = knock;
        stun = stu;
        // statusEffect = statusObject; //FIXME
    }

}



public delegate void ButtonPropertyEvent();
[System.Serializable]
public class ButtonProperty {
    public int fish; 

    public event ButtonPropertyEvent ButtonPressed;   


    public ButtonProperty(ButtonPropertyEvent method) {
        if (method != null)
            ButtonPressed += method;
    }

    public void RegsiterEvent(ButtonPropertyEvent method)
    {
        ButtonPressed += method;
    }

    public void UnregsiterEvent(ButtonPropertyEvent method)
    {
        ButtonPressed -= method;
    }

    public void Press() {
        ButtonPressed.Invoke(); 
    }
}



// [CustomPropertyDrawer (typeof (ButtonProperty))]
// class InteractiveObjectPropertyDrawer : PropertyDrawer {

//     public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {


//         EditorGUI.BeginProperty (position, label, property);

//         if(GUILayout.Button("Build Object"))
//         {

//             Debug.Log("BUTTON WORKSDDJS:");
//         }

//         EditorGUI.EndProperty ();
//     }
// }