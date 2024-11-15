using UnityEngine;
using System.Collections;

public class EnemyStateController : MonoBehaviour
{
    public enum EnemyState { WALKING, ATTACKING, DEATH }
    public EnemyComponent enemyComponent;
    public EnemyState currentState;
    public GameObject destroyParent;
    // Leg movement
    public Transform leftLegJoint;
    public Transform rightLegJoint;
    public float rotationAngle = 15f;
    public float rotationDuration = 0.5f;

    // Attack settings
    public float attackRange = 2f;
    private GameObject player;
    public float attackChargeUp = 1f;

    private Coroutine walkCoroutine;
    private float attackTimer;

    // Arm settings for Zombie Attack
    public Transform leftArm;
    public Transform rightArm;
    public float armRotationAngle = 30f;
    public float rotationDurationForArms = 0.1f;

    // Joint settings
    public ConfigurableJoint[] joints;
    public Transform[] Arm;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }

        TransitionToState(EnemyState.WALKING);
        StartCoroutine(WalkingState());
    }

    private void Update()
    {
        if (player != null)
        {
            CheckAttackRange();
        }
    }

    private void CheckAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (currentState != EnemyState.DEATH)
        {
            if (distance <= attackRange)
            {
                if (currentState != EnemyState.ATTACKING)
                {
                    TransitionToState(EnemyState.ATTACKING);
                }
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackChargeUp)
                {
                    ZombieAttack();
                    attackTimer = 0f;
                }
            }
            else
            {
                if (currentState == EnemyState.ATTACKING)
                {
                    TransitionToState(EnemyState.WALKING);
                }
                
                attackTimer = 0f;
            }
        }
    }

    private void ZombieAttack()
    {
        Debug.Log("Zombie attacks!");
        
        StartCoroutine(RotateArm(leftArm.transform, armRotationAngle));
        StartCoroutine(RotateArm(rightArm.transform, armRotationAngle));
        
        //Make player take damage
    }
    
    private IEnumerator RotateArm(Transform arm, float targetAngle)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = arm.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(targetAngle, 0, 0);
        while (elapsedTime < rotationDuration)
        {
            arm.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDurationForArms);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arm.localRotation = endRotation;
        elapsedTime = 0f;
        while (elapsedTime < rotationDuration)
        {
            arm.localRotation = Quaternion.Slerp(endRotation, startRotation, elapsedTime / rotationDurationForArms);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arm.localRotation = startRotation;
    }

    public void TransitionToState(EnemyState newState)
    {
        if (currentState == newState) return;

        StopCurrentState();
        currentState = newState;

        switch (currentState)
        {
            case EnemyState.WALKING:
                walkCoroutine = StartCoroutine(WalkingState());
                break;
            case EnemyState.ATTACKING:
                //Debug.Log("Enemy is attacking!");
                break;
            case EnemyState.DEATH:
                //Debug.Log("Enemy has died.");
                break;
        }
    }

    private void StopCurrentState()
    {
        if (walkCoroutine != null)
        {
            StopCoroutine(walkCoroutine);
            walkCoroutine = null;
        }
    }

    private IEnumerator WalkingState()
    {
        while (currentState == EnemyState.WALKING)
        {
            yield return RotateLeg(leftLegJoint, -rotationAngle);
            yield return RotateLeg(leftLegJoint, rotationAngle);
            yield return RotateLeg(rightLegJoint, -rotationAngle);
            yield return RotateLeg(rightLegJoint, rotationAngle);
        }
    }

    private IEnumerator RotateLeg(Transform leg, float targetAngle)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = leg.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(targetAngle, 0, 0);

        while (elapsedTime < rotationDuration)
        {
            leg.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        leg.localRotation = endRotation;
    }

    public void RagDoll(float xSpring, float yzSpring)
    {
        ResetRotations();
        foreach (ConfigurableJoint joint in joints)
        {
            if (joint != null)
            {
                JointDrive angularXDrive = joint.angularXDrive;
                angularXDrive.positionSpring = xSpring;
                joint.angularXDrive = angularXDrive;

                JointDrive angularYZDrive = joint.angularYZDrive;
                angularYZDrive.positionSpring = yzSpring;
                joint.angularYZDrive = angularYZDrive;
            }
        }
    }

    public void ResetRotations()
    {
        foreach (Transform jointTransform in Arm)
        {
            if (jointTransform != null)
            {
                jointTransform.localRotation = Quaternion.identity;
            }
        }
    }
}
