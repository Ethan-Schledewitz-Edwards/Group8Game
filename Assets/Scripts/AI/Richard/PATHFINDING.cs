using UnityEngine;
using UnityEngine.AI;

public class RagDollFollower : MonoBehaviour
{
    public Rigidbody rb;
    public float rotationSpeed = 5f;
    public float moveSpeed = 3f;

    private Transform player;
    private NavMeshPath path;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();

        // Find the player GameObject by tag and get its Transform
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the 'Player' tag assigned.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        UpdatePath();

        if (path.corners.Length > 1)
        {
            RotateTowardsTarget(path.corners[1]);
        }
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
}
