using UnityEngine;

public class PlayerComponent : EntityBase
{
    [field: SerializeField] public Transform DropPoint;

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
