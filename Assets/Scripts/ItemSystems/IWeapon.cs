
using UnityEngine;

public interface IWeapon
{
    public int Damage { get; }
    public int Durability { get; }
    public int MaxDurability { get; }

    #region Durability Methods
    public abstract void SetDurability(int value);

    public abstract void AddDurability(int value);

    public abstract void RemoveDurability(int value);
    #endregion

    #region Equip Methods
    public abstract void Equip(PlayerComponent playerComponent, Transform parent);

    public abstract void UnEquip();
    #endregion
}
