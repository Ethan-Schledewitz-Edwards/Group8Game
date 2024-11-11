using UnityEngine;
using UnityEngine.InputSystem;

public class HandVelocity : MonoBehaviour
{
    [SerializeField] InputActionProperty inputAction;

    public Vector3 Velocity { get; private set; }

    private void Update()
    {
        Velocity = inputAction.action.ReadValue<Vector3>();
    }
}
