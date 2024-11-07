using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Singleton")]
    public static GameManager Instance;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Enemies")]
    [SerializeField] private EnemyWeight[] enemies;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip roundStart;
    [SerializeField] private AudioClip gameOver;

    [Header("System")]
    private int waveNumber;
    private int enemiesPerWave;
    private EnemyWeight[] availableEnemies;

    #region Initialization Methods

    private void Awake()
    {
        // Define sigleton
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    private void Start()
    {
        StartGame();
    }
    #endregion

    #region Game Methods

    private void StartGame()
    {
        waveNumber = 1;
        enemiesPerWave = 5;

        FilterEnemies();

        // Start the first round
        StartCoroutine(WaveCooldown());
    }

    private void StartNewWave()
    {
        Debug.Log("Start Wave");

        for (int i = 0; i < enemiesPerWave; i++)
        {
            // Get an enemy prefab based on weighted probability
            GameObject enemyPrefab = GetWeightedEnemy();

            // Spawn the selected enemy at a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        }

        audioSource.PlayOneShot(roundStart);
    }

    private void EndWave()
    {
        Debug.Log($"Wave Complete: {waveNumber}");

        waveNumber++;
        enemiesPerWave += 2;

        // Filter out enemies not available in the current wave
        availableEnemies = enemies.Where(e => e.FloorWave <= waveNumber).ToArray();

        StartCoroutine(WaveCooldown());
    }

    private IEnumerator WaveCooldown()
    {
        float timer = 10f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            yield return null;
        }

        // Start the next wave after the cooldown
        StartNewWave();
    }

    /// <summary>
    /// THis method should be called when either the player dies (false), or when the player wins (true).
    /// </summary>
    /// <param name="isSucsessful"></param>
    public void EndGame(bool isSucsessful)
    {

    }
    #endregion

    #region Utility Methods

    /// <summary>
    /// Filters out enemies not available in the current wave
    /// </summary>
    private void FilterEnemies()
    {
        availableEnemies = enemies.Where(e => e.FloorWave <= waveNumber).ToArray();
    }

    public Transform GetRandomSpawn()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[spawnIndex];
    }

    /// <summary>
    /// Returns an enemy based on the weighted probability of it spawning
    /// </summary>
    public GameObject GetWeightedEnemy()
    {
        // Calculate the total weight of the enemy pool
        int totalWeight = availableEnemies.Sum(e => e.Weight);

        // Generate a random number within the range of total weights
        int randomWeight = Random.Range(0, totalWeight);
        foreach (var enemy in availableEnemies)
        {
            if (randomWeight < enemy.Weight)
            {
                return enemy.Prefab;
            }
            randomWeight -= enemy.Weight;
        }

        // Fallback
        return availableEnemies[0].Prefab;
    }

    #endregion
}

[System.Serializable]
public class EnemyWeight
{
    [field: SerializeField] public int FloorWave; // The lowest possible wave for this enemy type to appear
    [field: SerializeField] public int Weight { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
