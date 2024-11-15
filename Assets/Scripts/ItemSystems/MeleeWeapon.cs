using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable, RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MeleeWeapon : XRGrabInteractable, IWeapon
{
    [Header("Properties")]
    public int Damage => damage;
    [SerializeField] private int damage;

    public int Durability => durability;
    private int durability;

    public int MaxDurability => maxDurability;
    [SerializeField] private int maxDurability;

    [Header("Physics")]
    [SerializeField] private LayerMask damagingLayers;
    [SerializeField] private Transform sphereCastOrigin; // Radius of the sphere cast
    [SerializeField] private float sphereCastRadius = 0.1f; // Radius of the sphere cast
    [SerializeField] private float sphereCastDistance = 0.5f; // Distance to check ahead

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _impactSound;

    [Header("Components")]
    private Rigidbody rigidbody;
    private PlayerComponent player;
    private Collider collider;
    private HandVelocity handVel;

    [Header("System")]
    private float velMag;

    #region XR Callbacks

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        handVel = args.interactorObject.transform.GetComponent<HandVelocity>();

        // Clear parent
        // This will remove an item from the shelf
        transform.parent = null;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        handVel = null;
        velMag = 0;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (isSelected)
        {
            UpdateVelocity();
            CheckVictemOverlap();
        }
    }

    private void UpdateVelocity()
    {
        velMag = handVel ? handVel.Velocity.magnitude : 0;
    }

    private void CheckVictemOverlap()
    {
        Vector3 direction = handVel ? handVel.Velocity.normalized : transform.forward;

        // Perform the SphereCast
        if (Physics.SphereCast(sphereCastOrigin.position, sphereCastRadius, direction, out RaycastHit hitInfo, sphereCastDistance, damagingLayers))
        {
            if (hitInfo.transform.TryGetComponent<EnemyComponent>(out var enemyComponent))
                HitVictem(enemyComponent, velMag);
        }
    }
    #endregion

    #region Initialization Methods

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        SetDurability(maxDurability);
    }

    #endregion

    #region Unity Callbacks

    private void OnCollisionEnter(Collision other)
    {
        // Check if the object is on a damageable layer and has an EnemyComponent
        if ((damagingLayers.value & (1 << other.gameObject.layer)) != 0 && other.transform.TryGetComponent<EnemyComponent>(out var enemyComponent))
            HitVictem(enemyComponent, other.relativeVelocity.magnitude);
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
    private void HitVictem(EnemyComponent victem, float vel)
    {
        int totalDmg = Mathf.FloorToInt(Mathf.Clamp(damage * vel, 0, Damage));// Multiply damage by velocity
        int dur = totalDmg * 2;

        RemoveDurability(dur);
        victem.TakeDamage(totalDmg);

        // Play SFX
        if (_impactSound != null)
            _audioSource.PlayOneShot(_impactSound);
    }

    #endregion
}
