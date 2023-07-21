using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Pathfinding;
using System;

public abstract class EnemyBehaviour : MonoBehaviour, IDamageable, IKnockbackable, IStunnable
{
    public EnemyInfoObjectOld enemyInfoObject; 
    protected EnemyInfoOld info; 

    protected System.Random random_gen; 

    

    protected enum State {
        Idle, 
        Stunned, 
        Attack,
        Roam,
        Hunt,
        Die
    }

    protected Rigidbody2D _rigidbody2D; 

    protected int _health = 0; 
    [SerializeField] protected State _state = State.Roam; 

    [SerializeField] protected float attackTime = 0f; 
    [SerializeField] protected float roamTime = 0f; 
    [SerializeField] protected float huntTime = 0f; 

    protected float stunTime = 0f; 





    [SerializeField] protected float lookDistance = 10f; 


    protected const string TARGET_TAG = Constants.TAG_ENEMY_TARGET; 

    LayerMask zlayermask = (1 << 7 | 1 << 9); //Can only detect walls and players


    protected List<GameObject> hitboxes_go = new List<GameObject>(); 

    protected SeekerInstance seekerInstance; 
    [SerializeField]
    private Coroutine _routine; 

    protected AttributesController attributesController; 

    private void Awake() {

        random_gen = new System.Random();

        this.gameObject.AddTag(Constants.TAG_POOLED); 

        info = enemyInfoObject.info; 
        _rigidbody2D =  this.GetComponent<Rigidbody2D>();
        _rigidbody2D.mass = info.mass; 
        _rigidbody2D.drag = info.linear_drag; 
        _rigidbody2D.angularDrag = info.angular_drag; 
        SetupHitboxes();

        _health = info.hp; 

        Seeker seeker = this.GetComponent<Seeker>(); 
        if (seeker == null) {
            seeker = this.gameObject.AddComponent<Seeker>(); 
        }
        seekerInstance = new SeekerInstance(seeker); 


        if (_routine == null) {
            _routine = StartCoroutine(Routine()); 
        }

        attributesController = this.GetComponent<AttributesController>(); 
    }


    private void OnEnable() {
        _health = info.hp; 
        if (_routine == null) {
            _routine = StartCoroutine(Routine()); 
        }
    }

    
    void SetupHitboxes() {
        foreach (AttackInfoOld attack in info.attack_list) {
            GameObject go = new GameObject("Hitbox"); 
            // go.transform.SetParent(this.transform); 
            go.SetActive(false); 
            Hitbox_s hitbox_s = go.AddComponent<Hitbox_s>();
            hitbox_s.init(Constants.TAG_ENEMY_TARGET, attack.damageinfo, attack.duration, attack.repeat); 
            hitbox_s.SetupHitboxes(attack, this.transform); 

            // SpriteRenderer spren = go.AddComponent<SpriteRenderer>(); 
            // spren.sprite = attack.sprite; 

            hitboxes_go.Add(go); 


            // if (attack.shape == HitboxShape.Rectangle){
            //     BoxCollider2D col = go.AddComponent<BoxCollider2D>(); 
            //     col.size = attack.hitbox; 
            //     col.isTrigger = true; 

            // } else if (attack.shape == HitboxShape.Circle) {
            //     CircleCollider2D col = go.AddComponent<CircleCollider2D>(); 
            //     col.offset = attack.hitbox_offset; 
            //     col.radius = attack.hitbox[0]; 
            //     col.isTrigger = true; 
            // } 

            // if (attack.isBody) {
            //     go.transform.parent = this.transform; 
            //     go.transform.position = this.transform.position + attack.hitbox_offset.ToVector3().Rotate(this.transform.rotation.eulerAngles.z); 
            //     go.transform.localRotation =Quaternion.identity; 
            // }
        }
    }


    protected IEnumerator Routine() {
        while (true) {   
            // Debug.Log("routine"); 
            if (_health <= 0) {
                _health = 0; 
                _state = State.Die; 
            }     
            switch(_state) {
                case State.Attack:
                    AttackState(); 
                    yield return new WaitForSeconds(attackTime); 
                break; 
                case State.Idle:
                    yield return null; 
                break;
                case State.Roam:
                    RoamState(); 
                    yield return new WaitForSeconds(roamTime); 
                break;
                case State.Hunt:
                    HuntState(); 
                    yield return new WaitForSeconds(huntTime); 
                break;

                case State.Stunned:
                    StunState(); 
                    yield return new WaitForSeconds(stunTime); 
                    UnStun(); 
                break;
                case State.Die:
                    DieState(); 
                    yield break; 
                default:
                    yield return null; 
                break;
            }
            
        }
    }


    public abstract void AttackState();   
    public abstract void RoamState();   
    public abstract void HuntState();
    public abstract void StunState();


    public abstract void JustBeforeAttack(int num);
    public abstract void JustAfterAttack(int num);
    public abstract void AfterCooldown(int num);

    
    protected int GetRandomAttackNumber() {
        return random_gen.Next(info.attack_list.Count); 
    }

    protected void StartRandomAttack() {
        //TODO
        int randomattack = random_gen.Next(info.attack_list.Count); 
        StartAttack(randomattack);
    }


    protected void StartAttack(int num) {
        if (_state == State.Die) {
            return; 
        }
        StartCoroutine(AttackRoutine(num)); 
    }

    IEnumerator AttackRoutine(int num) {
        yield return new WaitForSeconds(info.attack_list[num].prep_time); 
        JustBeforeAttack(num);
        if (_state == State.Die) {
            yield break; 
        }


        Attack(num); 
        yield return new WaitForSeconds(info.attack_list[num].duration);
        JustAfterAttack(num); 
        yield return new WaitForSeconds(info.attack_list[num].cooldown_time); 
        AfterCooldown(num); 

        yield break; 
    }

    protected void Attack(int num) {
        // Debug.Log("Hitbox attack number " + num);
        hitboxes_go[num].SetActive(true); 

        if (!info.attack_list[num].isBody) {
            hitboxes_go[num].transform.position = this.transform.position + info.attack_list[num].hitbox.offset.ToVector3().Rotate(this.transform.rotation.eulerAngles.z); 
            hitboxes_go[num].transform.rotation = this.transform.rotation; 
        }
        
        hitboxes_go[num].GetComponent<Hitbox_s>().Attack();
        this.GetComponent<Rigidbody2D>().AddRelativeForce(info.attack_list[num].move, ForceMode2D.Impulse); 
    }




    public void Damage(int damage) {
        _health -= damage; 
        totaldamage += damage; 
    }


    
    void Update() {

        if  (timer < counttime) {
            timer  += Time.deltaTime; 
        } else if (totaldamage > 0) {
            GameObject floatingIcon = ObjectPooler.SharedInstance.GetPooledObject(Constants.FLOATING_ICON); 

            floatingIcon.SetActive(true); 
            floatingIcon.GetComponent<FloatingIconController>().StartFloatingText("" + totaldamage, 1f, this.transform.position + new Vector3(1, 1, 0), Color.red); 


            totaldamage = 0; 

        }
        
    }
    int totaldamage = 0;
    float timer = 0f; 
    float counttime = 0.3f; 




    public void Knockback(Vector2 dir) {
        _rigidbody2D.AddForce(dir, ForceMode2D.Impulse);
    }



    State before_stun;
    public void Stun(float time)
    {
        if (_state != State.Stunned && _state != State.Die) {
            before_stun = _state; 
        }
        _state = State.Stunned; 
        stunTime = time; 
    }

    void UnStun() {
        if (_state == State.Die) {
            return; 
        }
        if (before_stun == State.Attack) {
            before_stun = State.Hunt; 
        }
        _state = before_stun; 
    }


    



    public virtual void DieState() {
        Debug.Log("DIe"); 
        

        GameObject go = ObjectPooler.SharedInstance.GetPooledObject(Constants.ZOMBIE_BLOOD); 
        go.transform.position = this.transform.position; 
        go.SetActive(true); 



        
        for (int i = transform.childCount-1; i >= 0; --i) {

            if (transform.GetChild(i).gameObject.HasTag(Constants.TAG_BULLET)) {
                transform.GetChild(i).GetComponent<Bullet>().UnstickFromGameObject(); 
            }
        }

        this.gameObject.DestoyOrDeactivate();
    }




    protected static float VectorToAngle(Vector2 vect) { 
        return Vector2.SignedAngle(new Vector2(0, 1), vect.normalized);
    }


    protected bool CanSeeTarget(Vector3 lookDir, float lookDistance, string targetTag) {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, lookDir, lookDistance, zlayermask);
        if (hit.collider != null) {
            // CustomTag ctag = hit.collider.gameObject.GetComponent<CustomTag>();
            // if (ctag != null) {
            //     return ctag.HasTag(targetTag); 
            // }
            return hit.collider.gameObject.HasTag(targetTag); 
        }   
        return false;         
    }
    




    private void OnDisable() {
        seekerInstance.Disable(); 
        StopCoroutine(_routine);
        _routine = null;  
    }

}


public class SeekerInstance {

    public Seeker seeker;
    public Path path; 

    public float nextWayPointDistance = 0.5f; 

    public SeekerInstance (Seeker _seeker){
        seeker = _seeker; 
    }

    public void StartPath(Vector3 start, Vector3 end) {
        seeker.StartPath (start, end, OnPathComplete);
        curSeekerIndex = 0; 
    }

    public void OnPathComplete(Path p) {
        path = p; 
    }


    public void Disable() {
        seeker.pathCallback -= OnPathComplete; 
    }

    int curSeekerIndex = 0;

    public void UpdateWayPoint(Vector3 position) {
        if (path != null) {
            if (curSeekerIndex < path.vectorPath.Count ) {
                if (Vector2.Distance(position, path.vectorPath[curSeekerIndex]) < nextWayPointDistance) {
                    curSeekerIndex++;
                }
            }
        }
    }

    public bool IsStillPathing() {
        return  path != null && curSeekerIndex < path.vectorPath.Count; 
    }

    public Vector3 GetNextWayPoint() {
        if (path != null) {
            return path.vectorPath[curSeekerIndex];
        }

        Debug.LogError("This shouldn't happen"); 
        return Vector3.zero; 
    }


}

