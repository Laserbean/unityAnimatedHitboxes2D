using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Laserbean.General;

namespace Laserbean.AttackHitbox2D
{

public class HitboxesController : MonoBehaviour
{

    public AttackHitboxInfo HitboxInfo {
        get; private set; 
    }

    [Min(1)]
    [SerializeField] int maxHitboxes = 50; 

    [System.Serializable]
    class HitboxContainer {
        public GameObject gameObject;

        HitboxControllerOld _hitbox; 

        public HitboxControllerOld hitbox {
            get {
                if (_hitbox == null) {
                    _hitbox = gameObject.GetComponent<HitboxControllerOld>(); 
                }
                if (_hitbox == null) {
                    _hitbox = gameObject.AddComponent<HitboxControllerOld>(); 
                }
                return _hitbox; 
            }
        }


    }

    List<HitboxContainer> hitboxes = new List<HitboxContainer>(); 


    [EasyButtons.Button]
    void testAttack() {
        Attack(0, 0); 
    }

    public void Attack(float angle, float error = 0f) {
        if (HitboxInfo == null )    return; 

        for (int i = 0; i < HitboxInfo.bullets; i++) {
            HitboxAttack(GetPooledHitboxC(), angle + RandomStatic.RandomGaussian(-error, +error));
        }

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


    private HitboxContainer CreateHitbox() {
        HitboxContainer hitboxreturn = new HitboxContainer(); 

        if (HitboxInfo.prefab == null) {
            hitboxreturn.gameObject = new GameObject("hitboxtest"); 
        } else {
            hitboxreturn.gameObject = Instantiate(HitboxInfo.prefab); 
        }

        if (hitboxreturn.gameObject.GetComponent<CustomTag>() == null) {
            hitboxreturn.gameObject.AddComponent<CustomTag>(); 
        }
        hitboxreturn.gameObject.GetComponent<CustomTag>().AddTag(Constants.TAG_HITBOX);
        hitboxreturn.gameObject.tag = Constants.TAG_HITBOX;
        
        hitboxreturn.gameObject.transform.SetParent(this.transform); 
        hitboxreturn.gameObject.transform.localPosition = Vector3.zero; 
        hitboxreturn.gameObject.transform.SetParent(this.transform); 

        hitboxreturn.hitbox.Initialize(); 

        return hitboxreturn; 
    }


    HitboxContainer InstantiateHitbox() {
        if (hitboxes.Count >= maxHitboxes) {
            // Debug.LogError("Seriously Hitbox can't be created cause bigger than max".DebugColor(Color.red)); 
            return null; 
        }
        HitboxContainer hitboxc = new HitboxContainer(); 


        hitboxc = CreateHitbox(); 

        if (HitboxInfo.stain_info.stainType != StainType.Nothing) {
            var stainer =  hitboxc.gameObject.AddComponent<TriggerStainer>();

            stainer.stainType = HitboxInfo.stain_info.stainType;
            stainer.value = HitboxInfo.stain_info.value;
        }

        hitboxc.gameObject.transform.SetParent(this.transform); 

        hitboxc.hitbox.SetupHitbox(HitboxInfo, this.transform); 
        hitboxc.hitbox.SetBlacklist(blacklist_tags_list); 

        hitboxes.Add(hitboxc);
        return hitboxc; 
    }

    // [SerializeField]
    // AttackInfoObject attack; 


    public void SetHitbox(AttackHitboxInfo hitboxinfo) {

        if (hitboxinfo == null) {
            Debug.Log("Can't set null hitboxinfo".DebugColor("red")); 
        return; 
        }

        ClearHitboxContainers(); 


        HitboxInfo = hitboxinfo; 
    }

    // [EasyButtons.Button]
    // void updatehitbox() {
    //     if (Hitbox == null) {
    //         Debug.Log("HitboxInfo of hitboxes controller is null".DebugColor("blue")); 
    //         return; 
    //     }
    //     Debug.Log("HitboxInfo Setuped".DebugColor(Color.green)); 

    //     // hitbox.SetupHitbox(Hitbox); 
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

        angle = HitboxInfo.zeroRotation ? 0 : angle; 
        if (HitboxInfo.movementInfo.isBody) {
            hitboxc.hitbox.SetKinematic(true);


            hitboxc.gameObject.transform.position = position + HitboxInfo.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 

            // this.gameObject.transform.localPosition = Hitbox.local_position.ToVector3().Rotate(angle); 
            hitboxc.gameObject.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {

            hitboxc.hitbox.SetKinematic(false);
            hitboxc.gameObject.transform.position = position + HitboxInfo.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            hitboxc.gameObject.transform.rotation = Quaternion.Euler(0,0,angle); 
            hitboxc.gameObject.transform.SetParent(null); 
        }

    }


}

}