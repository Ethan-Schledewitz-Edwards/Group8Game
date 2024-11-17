using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    public enum EEnemyState { WALKING, ATTACKING, DEATH }
    public EnemyComponent enemyComponent;
    public EEnemyState currentState;
    public GameObject destroyParent;

    // Leg movement
    public Transform leftLegJoint;
    public Transform rightLegJoint;
    public float rotationAngle = 15f;
    public float rotationDuration = 0.5f;

    // Attack settings
    public float attackRange = 2f;
    private Transform player;
    public float attackChargeUp = 2f;

    // Arm settings for Zombie Attack
    public Transform leftArm;
    public Transform rightArm;
    public float armRotationAngle = 30f;
    public float rotationDurationForArms = 0.1f;

    // Joint settings
    public ConfigurableJoint[] joints;
    public Transform[] Arm;

    // Navigation
    public float rotationSpeed = 5f;
    public float moveSpeed = 3f;
    private NavMeshPath path;

    [Header("Attacks")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask damagingLayers;
    [SerializeField] private float castRadius = 2f;

    [Header("Components")]
    private Rigidbody rb;

    [Header("System")]
    private Coroutine walkCoroutine;
    private float attackTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the 'Player' tag assigned.");
        }

        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();

        TransitionToState(EEnemyState.WALKING);
        StartCoroutine(WalkingState());
    }

    private void Update()
    {
        if (player == null) return;

        UpdatePath();

        if (currentState == EEnemyState.WALKING && path.corners.Length > 1)
        {
            RotateTowardsTarget(path.corners[1]);
        }

        CheckAttackRange();
    }

    private void UpdatePath()
    {
        if (player == null) return;

        NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path);
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }

        MoveTowardsTarget(targetPosition);
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Vector3 movement = directionToTarget.normalized * (moveSpeed * Time.deltaTime);
            rb.MovePosition(transform.position + movement);
        }
    }

    private void CheckAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (currentState != EEnemyState.DEATH)
        {
            if (distance <= attackRange)
            {
                if (currentState != EEnemyState.ATTACKING)
                {
                    TransitionToState(EEnemyState.ATTACKING);
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
                if (currentState == EEnemyState.ATTACKING)
                {
                    TransitionToState(EEnemyState.WALKING);
                }

                attackTimer = 0f;
            }
        }
    }

    private void ZombieAttack()
    {
        StartCoroutine(RotateArm(leftArm.transform, armRotationAngle));
        StartCoroutine(RotateArm(rightArm.transform, armRotationAngle));

        PlayerComponent.Instance.TakeDamage(2);
    }

    private IEnumerator RotateArm(Transform arm, float targetAngle)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = arm.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(targetAngle, 0, 0);
        while (elapsedTime < rotationDurationForArms)
        {
            arm.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDurationForArms);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arm.localRotation = endRotation;
        elapsedTime = 0f;
        while (elapsedTime < rotationDurationForArms)
        {
            arm.localRotation = Quaternion.Slerp(endRotation, startRotation, elapsedTime / rotationDurationForArms);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arm.localRotation = startRotation;
    }

    public void TransitionToState(EEnemyState newState)
    {
        if (currentState == newState) return;

        StopCurrentState();
        currentState = newState;

        switch (currentState)
        {
            case EEnemyState.WALKING:
                walkCoroutine = StartCoroutine(WalkingState());
                break;
            case EEnemyState.ATTACKING:
                break;
            case EEnemyState.DEATH:
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
        while (currentState == EEnemyState.WALKING)
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

    #region Debug

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if(attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(attackPoint.position, castRadius);
        }
    }
#endif
    #endregion
}