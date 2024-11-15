using UnityEngine;

public class PlayerComponent : EntityBase
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] _deathSounds;

    public override void Die()
    {
        Debug.Log("Do Die Things Lmao");

        if (_deathSounds != null && _deathSounds.Length > 0)
            audioSource.PlayOneShot(_deathSounds[Random.Range(0, _deathSounds.Length)]);
    }
}
