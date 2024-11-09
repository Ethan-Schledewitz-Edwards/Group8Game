using System;
using UnityEngine;

public class EnemyComponent : EntityBase
{
    [Header("Events")]
    public Action<EnemyComponent> OnDeath;

    public override void Die()
    {
        OnDeath?.Invoke(this);
    }
}
