using TMPro;
using UnityEngine;

public class WaveTextUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    private const string wave = "Wave";

    public void SetWave(int waveNumber)
    {
        waveText.text = $"{wave} {waveNumber}";

        animator.SetTrigger("StartWave");
    }
}
