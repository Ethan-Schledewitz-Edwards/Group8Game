using UnityEngine;
using System.Collections;

public class EnemyStateController : MonoBehaviour
{
    public enum EnemyState { WALKING, ATTACKING, DEATH }
    public EnemyState currentState;

    // Leg movement
    public Transform leftLegJoint;
    public Transform rightLegJoint;
    public float rotationAngle = 15f;
    public float rotationDuration = 0.5f;

    private Coroutine walkCoroutine;

    // Joint settings
    public ConfigurableJoint[] joints;
    public Transform[] Arm;

    private void Start()
    {
        TransitionToState(EnemyState.WALKING);
        StartCoroutine(WalkingState());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SwitchToNextState();
        }
    }

    private void SwitchToNextState()
    {
        if (currentState == EnemyState.WALKING)
        {
            TransitionToState(EnemyState.ATTACKING);
        }
        else if (currentState == EnemyState.ATTACKING)
        {
            TransitionToState(EnemyState.DEATH);
        }
        else if (currentState == EnemyState.DEATH)
        {
            TransitionToState(EnemyState.WALKING);
        }
    }

    private void TransitionToState(EnemyState newState)
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
                Debug.Log("Enemy is attacking!");
                break;
            case EnemyState.DEATH:
                Die();
                ResetRotations();
                Debug.Log("Enemy has died.");
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
            yield return RotateLeg(leftLegJoint, rotationAngle);
            yield return RotateLeg(leftLegJoint, -rotationAngle);
            yield return RotateLeg(rightLegJoint, rotationAngle);
            yield return RotateLeg(rightLegJoint, -rotationAngle);
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

    public void UpdateJointSprings(float xSpring, float yzSpring)
    {
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

    public void Die()
    {
        UpdateJointSprings(20f, 20f); 
        Destroy(gameObject, 3f);
    }
}
