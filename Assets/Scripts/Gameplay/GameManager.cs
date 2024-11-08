using System;
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

    private int waveSize; // The total amount of enemies this wave
    private int waveEnemyCap; // The amount of enemies allowed on screen
    private int enemyPool;// Enemies remaining

    private int kills;

    private EnemyWeight[] availableEnemies;

    [Header("Events")]
    public Action OnWaveFinish;

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

        waveSize = 5;
        waveEnemyCap = 3;
        enemyPool = waveSize;

        FilterEnemies();

        // Start the first round
        StartCoroutine(WaveCooldown());
    }

    private void StartNewWave()
    {
        Debug.Log("Start Wave");

        for (int i = 0; i < waveEnemyCap; i++)
        {
            // Get an enemy prefab based on weighted probability
            SpawnEnemy();
        }

        audioSource.PlayOneShot(roundStart);
    }

    private void EndWave()
    {
        Debug.Log($"Wave Complete: {waveNumber}");

        OnWaveFinish.Invoke();

        waveNumber++;

        waveSize += 1;
        waveEnemyCap += 1;
        enemyPool = waveSize;

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
    /// This method should be called when either the player dies (false), or when the player wins (true).
    /// </summary>
    /// <param name="isSucsessful"></param>
    public void EndGame(bool isSucsessful)
    {

    }
    #endregion

    #region Enemy Methods

    public void SpawnEnemy()
    {
        // Spawn the selected enemy at a random spawn point
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        EnemyComponent selectedEnemy = Instantiate(GetWeightedEnemy(), spawnPoints[spawnIndex].position, Quaternion.identity);
        selectedEnemy.OnDeath += KillEnemy;
        enemyPool--;
    }

    private EnemyComponent GetWeightedEnemy()
    {
        // Calculate the total weight of the enemy pool
        int totalWeight = availableEnemies.Sum(e => e.Weight);

        // Generate a random number within the range of total weights
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        foreach (var enemy in availableEnemies)
        {
            if (randomWeight < enemy.Weight)
            {
                return enemy.Prefab;
            }
            randomWeight -= enemy.Weight;
        }

        enemyPool--;

        // Fallback
        return availableEnemies[0].Prefab;
    }

    private void KillEnemy(EnemyComponent enemy)
    {
        Debug.Log("Enemy Died");

        enemy.OnDeath -= KillEnemy;

        // Check if an enemy can take the victems place
        if (enemyPool > 0)
            SpawnEnemy();

        kills++;
        if (kills >= waveSize)
            EndWave();
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
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        return spawnPoints[spawnIndex];
    }
    #endregion
}

[Serializable]
public class EnemyWeight
{
    [field: SerializeField] public int FloorWave; // The lowest possible wave for this enemy type to appear
    [field: SerializeField] public int Weight { get; private set; }
    [field: SerializeField] public EnemyComponent Prefab { get; private set; }
}
