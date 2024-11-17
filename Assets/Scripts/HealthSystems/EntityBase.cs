using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IHealthComponent, IDamagable
{
    [Header("Health")]
    public int Health => health;
    private int health;

    public int MaxHealth => maxHealth;
    [SerializeField] private int maxHealth;

    public bool IsDead => isDead;
    private bool isDead;

    public bool IsDamagable => isDamagable;
    [SerializeField] protected bool isDamagable;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip[] _hurtSounds;

    #region Initialization Methods

    protected virtual void Awake()
    {
        SetHealth(maxHealth);
    }

    #endregion

    #region Health Methods

    public void SetHealth(int value)
    {
        health = Mathf.Clamp(value, 0, maxHealth);

        if (!isDead)
        {
            isDead = health <= 0;

            if (isDead)
                Die();
        }
    }

    public void AddHealth(int value)
    {
        SetHealth(health + value);
    }

    public void RemoveHealth(int value)
    {
        SetHealth(health - value);
    }

    public void TakeDamage(int amount)
    {
        if (IsDamagable)
        {
            RemoveHealth(amount);

            // Spawn particle effects

            // Play Audio
            if (_hurtSounds != null && _hurtSounds.Length > 0)
                audioSource.PlayOneShot(_hurtSounds[Random.Range(0, _hurtSounds.Length)]);
        }
    }

    public abstract void Die();
    #endregion
}
