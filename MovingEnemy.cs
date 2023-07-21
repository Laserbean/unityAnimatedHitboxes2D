using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEnemy : EnemyBehaviour
{

    [SerializeField] protected float roam_speed;
    [SerializeField] protected float hunt_speed;
    [SerializeField] protected float rotateSpeed = 1f;


    protected MoveTarget moveTarget; 

    
    void Start()
    {
        moveTarget = new MoveTarget(this.transform.position);
        // moveTarget = this.transform.position; 

        seekerInstance.nextWayPointDistance = 2f; 
        _state = State.Roam; 
    }


    protected bool IsEnemyInRange(Vector3 targetPosition, float attackDistanceRange) {
        return (targetPosition - this.transform.position).sqrMagnitude < Mathf.Pow(attackDistanceRange, 2);        
    }


    protected void FixedUpdateRotation() {
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation,
                    moveTarget.GetRotationQuaternion(this.transform.position), 
                    Time.deltaTime * rotateSpeed);
    }


    protected void StepTowardsTarget(float speed) {
        if (attributesController != null) {
            speed += speed * AttributeCalculations.CalculateAgility(attributesController.GetAttributeValue(AttributeType.Agility));
        }
        _rigidbody2D.AddForce((moveTarget.position - this.transform.position.ToVector2()).normalized * speed, ForceMode2D.Impulse); 
    }

    protected class MoveTarget {
        public GameObject target = null; 
        public Vector2 position;
        public bool isActive = false; 
        
        public Quaternion GetRotationQuaternion(Vector3 thisPosition) {
            return Quaternion.Euler(new Vector3(0,0, VectorToAngle(position.ToVector3() - thisPosition)));
        }

        public MoveTarget(Vector3 targ) {
            position = (Vector2) targ; 
        }
    }

}
