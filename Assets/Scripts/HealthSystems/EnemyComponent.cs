using System;
using System.Collections;
using UnityEngine;

public class EnemyComponent : EntityBase
{
    [Header("Events")]
    public Action<EnemyComponent> OnDeath;
    public EnemyAI enemyAI;

    [SerializeField] private LayerMask weaponLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _ambientMoans;

    [Header("System")]
    private bool isCounting = true;
    private float timer;

    #region Initialization Methods

    private void Awake()
    {
        timer = UnityEngine.Random.Range(5, 15);
        isCounting = true;
    }
    #endregion

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
                    timer = UnityEngine.Random.Range(5, 15);
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
            if (other.TryGetComponent<MeleeWeapon>(out var weapon))
                weapon.HitVictem(this);
        }
    }
    #endregion

    public override void Die()
    {
        // Stop moans on death
        isCounting = false;

        OnDeath?.Invoke(this);
        enemyAI.TransitionToState(EnemyAI.EEnemyState.DEATH);
        Ragdoll();
    }

    private void Ragdoll()
    {
        float xSpring = 20f;
        float yzSpring = 20f;
        enemyAI.RagDoll(xSpring, yzSpring);
        
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
