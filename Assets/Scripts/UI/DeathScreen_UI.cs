using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class DeathScreen_UI : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _timerText;
    private const string messageText = "Returning to main menu in";

    [Header("System")]
    private bool isCounting;
    private float timer;

    #region Initialization Methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    #endregion

    #region Animation Events

    /// <summary>
    /// Called by the game over aniamtion
    /// </summary>
    public void StartCount()
    {
        isCounting = true;
    }
    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (isCounting)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;

                _timerText.text = $"{messageText} {Mathf.FloorToInt(timer)}";

                // Return to main menu after timer runs out
                if (timer <= 0)
                    MainMenu();
            }
        }
    }

    #endregion

    public void TriggerGameOver()
    {
        animator.SetTrigger("GameOver");
    }

    private void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
