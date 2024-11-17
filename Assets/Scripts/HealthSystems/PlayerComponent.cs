using UnityEngine;

public class PlayerComponent : EntityBase
{
    [Header("Singleton")]
    public static PlayerComponent Instance;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _deathSounds;
    [SerializeField] private AudioClip[] _stepSounds;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public override void Die()
    {
        isDamagable = false;

        // Trigger game over screen
        UIManager.Instance.DeathScreen.TriggerGameOver();

        // Play death sounds
        if (_deathSounds != null && _deathSounds.Length > 0)
            audioSource.PlayOneShot(_deathSounds[Random.Range(0, _deathSounds.Length)]);
    }
}
