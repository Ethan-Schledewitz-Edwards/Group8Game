using System;
using System.Collections;
using UnityEngine;

public class EnemyComponent : EntityBase
{
    [Header("Events")]
    public Action<EnemyComponent> OnDeath;
    public EnemyStateController enemyStateController;

    [SerializeField] private LayerMask weaponLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _ambientMoans;

    [Header("System")]
    private bool isCounting;
    private float timer = 10f;

    #region Unity Callbacks
    private void Update()
    {
        if (isCounting)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                // Play moan
                if (timer <= 1)
                {
                    if (_ambientMoans != null && _ambientMoans.Length > 0)
                        audioSource.PlayOneShot(_ambientMoans[UnityEngine.Random.Range(0, _ambientMoans.Length)]);

                    // Reset timer
                    timer = 10f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is on the weapon layer
        if ((weaponLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            // Get the Weapon component if needed, or just handle the hit directly
            if (other.TryGetComponent<IWeapon>(out var weapon))
                TakeDamage(weapon.Damage);
        }
    }
    #endregion

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
