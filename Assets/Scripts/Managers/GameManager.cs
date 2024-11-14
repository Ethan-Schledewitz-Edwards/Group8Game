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
    private int waveSize;
    private int waveEnemyCap;
    private int enemyPool;
    private int killsInWave;
    private int currentEnemiesOnScreen;

    private EnemyWeight[] availableEnemies;

    [Header("Events")]
    public Action OnWaveFinish;

    #region Initialization Methods

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
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

        StartCoroutine(WaveCooldown());
    }

    private void StartNewWave()
    {
        Debug.Log("Start Wave " + waveNumber);

        FilterEnemies();
        killsInWave = 0;
        currentEnemiesOnScreen = 0;

        // Spawn initial wave enemies
        for (int i = 0; i < Math.Min(waveEnemyCap, enemyPool); i++)
            SpawnEnemy();

        audioSource.PlayOneShot(roundStart);
    }

    private void EndWave()
    {
        Debug.Log($"Wave Complete: {waveNumber}");
        OnWaveFinish?.Invoke();

        waveNumber++;
        waveSize += 1 * waveNumber;
        waveEnemyCap += 1;
        enemyPool = waveSize;

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
        StartNewWave();
    }

    public void EndGame(bool isSucsessful)
    {
        // Handle game end
    }

    #endregion

    #region Enemy Methods

    public void SpawnEnemy()
    {
        if (enemyPool <= 0 || currentEnemiesOnScreen >= waveEnemyCap)
            return;

        Transform spawn = GetRandomSpawn();
        EnemyComponent selectedEnemy = Instantiate(GetWeightedEnemy(), spawn.position, Quaternion.identity);
        selectedEnemy.OnDeath += EnemyDied;

        enemyPool--;
        currentEnemiesOnScreen++;
    }

    private EnemyComponent GetWeightedEnemy()
    {
        int totalWeight = availableEnemies.Sum(e => e.Weight);
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);

        foreach (var enemy in availableEnemies)
        {
            if (randomWeight < enemy.Weight)
                return enemy.Prefab;
            randomWeight -= enemy.Weight;
        }

        return availableEnemies[0].Prefab;
    }

    private IEnumerator EnemyRespawnCooldown()
    {
        float timer = UnityEngine.Random.Range(4, 7);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        SpawnEnemy();
    }

    private void EnemyDied(EnemyComponent enemy)
    {
        Debug.Log("Enemy Died");

        enemy.OnDeath -= EnemyDied;
        currentEnemiesOnScreen--;

        killsInWave++;
        if (killsInWave >= waveSize)
        {
            EndWave();
        }
        else if (enemyPool > 0 && currentEnemiesOnScreen < waveEnemyCap)
        {
            StartCoroutine(EnemyRespawnCooldown());
        }
    }

    #endregion

    #region Utility Methods

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
