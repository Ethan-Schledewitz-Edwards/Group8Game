using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    [field: SerializeField] public WaveTextUI WaveText { get; private set; }
    [field: SerializeField] public DeathScreen_UI DeathScreen { get; private set; }
}
