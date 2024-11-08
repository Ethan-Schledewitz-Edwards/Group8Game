public interface IHealthComponent
{
    public int Health { get; }
    public int MaxHealth { get; }
    public bool IsDead { get; }

    #region Methods
    public abstract void SetHealth(int value);

    public abstract void AddHealth(int value);

    public abstract void RemoveHealth(int value);

    public abstract void Die();
    #endregion
}