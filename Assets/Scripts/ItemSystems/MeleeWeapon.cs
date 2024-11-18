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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] pickupSounds;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip breakSound;

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
                HitVictem(enemyComponent);
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

    #region Durability Methods
    public void SetDurability(int value)
    {
        durability = Mathf.Clamp(value, 0, maxDurability);

        // Break the weapon if the durability is reduced to zero
        if (durability <= 0)
            BreakWeapon();
    }

    public void AddDurability(int value)
    {
        SetDurability(durability + value);
    }

    public void RemoveDurability(int value)
    {
        SetDurability(durability - value);
    }

    public void BreakWeapon()
    {
        Destroy(gameObject);

        // Play SFX
        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);
    }
    #endregion

    #region Equip Methods

    public void Equip(PlayerComponent playerComponent, Transform parent)
    {
        rigidbody.useGravity = false;
        player = playerComponent;

        // Play pickup sound
        if (pickupSounds != null && pickupSounds.Length > 0)
            audioSource.PlayOneShot(pickupSounds[UnityEngine.Random.Range(0, pickupSounds.Length)]);

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
    public void HitVictem(EnemyComponent victem)
    {
        int totalDmg = Mathf.FloorToInt(Mathf.Clamp(damage * velMag, 0, Damage));// Multiply damage by velocity
        int dur = totalDmg * 2;

        RemoveDurability(dur);
        victem.TakeDamage(totalDmg);

        Debug.Log("WTF");
        audioSource.PlayOneShot(impactSound);
    }
    #endregion
}
