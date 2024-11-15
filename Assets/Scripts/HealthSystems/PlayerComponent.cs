using UnityEngine;

public class PlayerComponent : EntityBase
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] _deathSounds;
    [SerializeField] private AudioClip[] _stepSounds;


    public override void Die()
    {
        // Trigger game over screen
        UIManager.Instance.DeathScreen.TriggerGameOver();

        // Play death sounds
        if (_deathSounds != null && _deathSounds.Length > 0)
            audioSource.PlayOneShot(_deathSounds[Random.Range(0, _deathSounds.Length)]);
    }
}
