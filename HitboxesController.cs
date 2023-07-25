using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxesController : MonoBehaviour
{
    // [SerializeField]
    HitboxInfo hitbox_info; 

    public HitboxInfo hitbox {
        get {return hitbox_info;}
    }

    [Min(1)]
    [SerializeField] int maxHitboxes = 1; 

    [System.Serializable]
    class HitboxContainer {
        public GameObject gameObject;

        Hitbox _hitbox; 

        public Hitbox hitbox {
            get {
                if (_hitbox == null) {
                    _hitbox = gameObject.GetComponent<Hitbox>(); 
                }
                if (_hitbox == null) {
                    _hitbox = gameObject.AddComponent<Hitbox>(); 
                }
                return _hitbox; 
            }
        }


    }
    GameObject hitbox_OB; 
    Hitbox hitbox_s; 

    List<HitboxContainer> hitboxes = new List<HitboxContainer>(); 


    private void Start() {
        hitbox_OB = new GameObject("hitboxtest"); 
        hitbox_OB.transform.SetParent(this.transform); 
        hitbox_OB.transform.localPosition = Vector3.zero; 

        hitbox_s = hitbox_OB.AddComponent<Hitbox>(); 
        hitbox_s.awake(); 

        hitbox_OB.transform.SetParent(this.transform); 
    }

    [EasyButtons.Button]
    void testAttack() {
        Attack(0); 
    }

    public void Attack(float angle) {
        HitboxAttack(GetPooledHitboxC(), angle); 
    }

    HitboxContainer GetPooledHitboxC() {
        if (hitboxes.Count == 0) {
            return InstantiateHitbox();
        }

        foreach (var hitboxc in hitboxes) {
            if (!hitboxc.gameObject.activeSelf) {
                return hitboxc; 
            }
        }

        return InstantiateHitbox();
    }

    void HitboxAttack(HitboxContainer hitboxC, float angle) {
        if (hitboxC == null) return; 
        ResetHitboxPosition(hitboxC, angle, this.transform.position);
        hitboxC.hitbox.Attack(angle);         
    }

    #region  hitbox_setup
    List<string> blacklist_tags_list = new List<string>();
    public void AddBlacklist(string tagg) {
        if (blacklist_tags_list.Contains(tagg)) return; 
        blacklist_tags_list.Add(tagg); 
    }



    HitboxContainer InstantiateHitbox() {
        if (hitboxes.Count >= maxHitboxes) {
            Debug.LogError("Seriously Hitbox can't be created cause bigger than max".DebugColor(Color.red)); 
            return null; 
        }
        HitboxContainer hitboxc = new HitboxContainer(); 
        hitboxc.gameObject = Instantiate(hitbox_OB, this.transform.position, Quaternion.identity);
        hitboxc.gameObject.transform.SetParent(this.transform); 

        hitboxc.hitbox.SetupHitbox(hitbox_info); 
        hitboxc.hitbox.SetBlacklist(blacklist_tags_list); 

        hitboxes.Add(hitboxc);
        return hitboxc; 
    }

    // [SerializeField]
    // AttackInfoObject attack; 


    public void SetHitbox(HitboxInfo hitboxinfo) {

        if (hitboxinfo == null) {
            Debug.Log("Can't set null hitboxinfo".DebugColor("red")); 
        return; 
        }

        ClearHitboxContainers(); 
        hitbox_info = hitboxinfo; 
    }

    // [EasyButtons.Button]
    // void updatehitbox() {
    //     if (hitbox_info == null) {
    //         Debug.Log("HitboxInfo of hitboxes controller is null".DebugColor("blue")); 
    //         return; 
    //     }
    //     Debug.Log("HitboxInfo Setuped".DebugColor(Color.green)); 

    //     // hitbox.SetupHitbox(hitbox_info); 
    //     ClearHitboxContainers(); 
    // }
    
    void ClearHitboxContainers() {
        HitboxContainer[] toDelete = hitboxes.ToArray(); 

        for (int i = toDelete.Length -1; i >= 0; i--) {
            Destroy(toDelete[i].gameObject); 
        }

        hitboxes.Clear(); 
    }

    #endregion

    void ResetHitboxPosition(HitboxContainer hitboxc, float angle, Vector3 position) {

        angle = hitbox_info.zeroRotation ? 0 : angle; 
        if (hitbox_info.isBody) {
            hitboxc.gameObject.transform.position = position + hitbox_info.local_position.ToVector3().Rotate(angle); 

            // this.gameObject.transform.localPosition = hitbox_info.local_position.ToVector3().Rotate(angle); 
            hitboxc.gameObject.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {
            hitboxc.gameObject.transform.position = position + hitbox_info.local_position.ToVector3().Rotate(angle); 
            hitboxc.gameObject.transform.rotation = Quaternion.Euler(0,0,angle); 
        }
    }




}