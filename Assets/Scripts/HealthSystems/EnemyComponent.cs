using System;
using System.Collections;
using UnityEngine;

public class EnemyComponent : EntityBase
{
    [Header("Events")]
    public Action<EnemyComponent> OnDeath;
    public EnemyStateController enemyStateController;
    
    public override void Die()
    {
        OnDeath?.Invoke(this);
        enemyStateController.TransitionToState(EnemyStateController.EnemyState.DEATH);
        Ragdoll();
    }

    private void Ragdoll()
    {
        float xSpring = 20f;
        float yzSpring = 20f;
        enemyStateController.RagDoll(xSpring, yzSpring);
        
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

        Destroy(transform.root.gameObject);
    }
}
