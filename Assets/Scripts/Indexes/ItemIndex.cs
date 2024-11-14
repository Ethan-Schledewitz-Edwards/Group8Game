using UnityEngine;

[CreateAssetMenu(fileName = "ItemIndex", menuName = "Items/ItemIndex", order = 1)]
public class ItemIndex : ScriptableObject
{
    [field: SerializeField] public MeleeWeapon[] WeaponIndex { get; private set; }
}
