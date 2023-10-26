using System.Collections;
using System.Collections.Generic;
using Laserbean.General;
using UnityEngine;

using Laserbean.Colliders.Hitbox2d;

namespace Laserbean.AttackHitbox2D {

public class HitboxManager : MonoBehaviour
{    
    MiniObjectPooler hitboxpooler; 

    AttackHitboxInfo HitboxInfo;

    [SerializeField]
    List<string> blacklist_tags_list = new List<string>();
    
    private void Start() {
        hitboxpooler = new (null, transform); 
    }

    [EasyButtons.Button] 
    public void SetHitbox(AttackInfoObject attackInfoObject) {
        SetHitbox(attackInfoObject.attack.hitboxes[0]); 
    }

    public void SetHitbox(AttackHitboxInfo hitboxinfo) {
        HitboxInfo = hitboxinfo;



        var hitboxControllerNew = HitboxCreator.CreateHitbox(hitboxinfo.HitboxShapeInfo, hitboxinfo.rigidbodyInfo); 
        var go = hitboxControllerNew.gameObject;
        go.transform.SetParent(transform); 
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        var attackcomp = go.AddComponent<HitboxAttackComponent>(); 
        attackcomp.SetBlacklist(blacklist_tags_list);
        attackcomp.SetUp(hitboxinfo);

        go.SetActive(false); 

        ObjectPoolItem poolItem = new (go, hitboxinfo.bullets*2, true); 

        if (hitboxpooler == null) {
            hitboxpooler = new(poolItem, transform); 
        } else {
            hitboxpooler.DestroyAllPooledObjects(); 
            hitboxpooler.SetPoolItem(poolItem, transform); 
        }
    }

    [EasyButtons.Button]
    public void Attack(float angle, float error = 0f) {
        if (HitboxInfo == null )    return; 

        for (int i = 0; i < HitboxInfo.bullets; i++) {
            HitboxAttack(GetHitboxFromPooler(), angle + RandomStatic.RandomGaussian(-error, +error));
        }
    }

    GameObject GetHitboxFromPooler() {
        var go = hitboxpooler.GetPooledObject(); 
        var attackcomp = go.GetComponent<HitboxAttackComponent>(); 
        attackcomp.SetBlacklist(blacklist_tags_list);
        attackcomp.SetUp(HitboxInfo);      

        return go; 
    }


    void HitboxAttack(GameObject hitboxC, float angle) {
        if (hitboxC == null) return; 

        ResetHitboxPosition(hitboxC, angle, transform.position);
        hitboxC.SetActive(true); 
        StartCoroutine(HitboxAttackRoutine(hitboxC, angle));
    }

    IEnumerator HitboxAttackRoutine(GameObject hitboxC, float angle) {
        var hitboxattack = hitboxC.GetComponent<HitboxAttackComponent>(); 
        yield return hitboxattack.Attack(angle);         
        hitboxattack.gameObject.transform.SetParent(transform); 
        yield break; 
    }


    void ResetHitboxPosition(GameObject hitboxc, float angle, Vector3 position) {
        var hitboxcontroller = hitboxc.GetComponent<HitboxControllerNew>(); 

        angle = HitboxInfo.zeroRotation ? 0 : angle; 
        if (HitboxInfo.movementInfo.isBody) {
            hitboxcontroller.SetKinematic(true);
            hitboxc.transform.position = position + HitboxInfo.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            hitboxc.transform.rotation = Quaternion.Euler(0,0,angle);
        } else {

            hitboxcontroller.SetKinematic(false);
            hitboxc.transform.position = position + HitboxInfo.HitboxShapeInfo.local_position.ToVector3().Rotate(angle); 
            hitboxc.transform.rotation = Quaternion.Euler(0,0,angle); 
            hitboxc.transform.SetParent(null); 
        }

    }



}

}
