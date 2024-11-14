using System;
using System.Collections;
using UnityEngine;

public class EnemyComponent : EntityBase
{
    [Header("Events")]
    public Action<EnemyComponent> OnDeath;

    public override void Die()
    {
        OnDeath?.Invoke(this);

        Ragdoll();
    }

    private void Ragdoll()
    {
        // Ragdoll the mesh

        StartCoroutine(DespawnTimer());
    }

    private IEnumerator DespawnTimer()
    {
        float timer = 10f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
