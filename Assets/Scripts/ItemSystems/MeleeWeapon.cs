using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MeleeWeapon : MonoBehaviour, IWeapon
{
    [Header("Properties")]
    public int Damage => damage;
    [SerializeField] private int damage;

    public int Durability => durability;
    private int durability;

    public int MaxDurability => maxDurability;
    [SerializeField] private int maxDurability;

    [SerializeField] private LayerMask damagingLayers;

    [Header("Components")]
    private Rigidbody rigidbody;
    private PlayerComponent player;
    private Collider collider;

    #region Initialization Methods

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        SetDurability(maxDurability);
    }

    #endregion

    #region Unity Callbacks

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other);

        // Check if the object is on a damageable layer and has an EnemyComponent
        if ((damagingLayers.value & (1 << other.gameObject.layer)) != 0 && other.transform.TryGetComponent<EnemyComponent>(out var enemyComponent))
            HitVictem(enemyComponent);
    }
    #endregion

    #region Durability Methods
    public void SetDurability(int value)
    {
        durability = value;
    }

    public void AddDurability(int value)
    {
        SetDurability(durability + value);
    }

    public void RemoveDurability(int value)
    {
        SetDurability(durability - value);
    }
    #endregion

    #region Equip Methods

    public void Equip(PlayerComponent playerComponent, Transform parent)
    {
        rigidbody.useGravity = false;
        player = playerComponent;

        transform.parent = parent;
    }

    public void UnEquip()
    {
        rigidbody.useGravity = false;

        transform.parent = null;
        player = null;
    }
    #endregion

    #region Damage Methods


    /// <summary>
    /// Applies a weapons damage to a victem and degrades the weapon.
    /// </summary>
    private void HitVictem(EnemyComponent victem)
    {
        float vel = rigidbody.velocity.magnitude;
        int totalDmg = Mathf.FloorToInt(Mathf.Clamp(damage * vel, 0, Damage));// Multiply damage by velocity
        int dur = totalDmg * 2;

        RemoveDurability(dur);
        victem.TakeDamage(totalDmg);
    }

    #endregion
}
