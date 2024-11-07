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
    [SerializeField] private bool isDamagable;

    #region Health Methods

    public void SetHealth(int value)
    {
        health = Mathf.Clamp(value, 0, maxHealth);

        // Update HUD
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
        RemoveHealth(amount);

        // Spawn particle effects
    }
    #endregion
}
