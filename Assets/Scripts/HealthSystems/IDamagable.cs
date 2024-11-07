using UnityEngine;

public interface IDamagable
{
    public bool IsDamagable { get; }

    public void TakeDamage(int amount);
}
