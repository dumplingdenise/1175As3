using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{

    // singleton pattern
    public static WaveManager Instance { get; private set; }

    [Tooltip("Assign your generic enemy prefab here.")]
    public GameObject enemyPrefab; // Assign your generic enemy prefab here in the Inspector

    [Tooltip("Assign GameObjects here that will serve as spawn points for enemies.")]
    public Transform[] spawnPoints; // assign in the Inspector

    private int currentWaveIndex = -1; // -1 means no wave has started yet
    private Coroutine currentWaveCoroutine; // reference to the active wave spawning coroutine
    private int enemiesRemainingInWave = 0; // tracks enemies to know when a wave is cleared

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // If you want WaveManager to persist across scenes, uncomment the line below.
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ensure DataManager has loaded data before trying to access it
        // This check is important due to script execution order
        if (DataManager.Instance != null && DataManager.Instance.allEnemyWaves != null && DataManager.Instance.allEnemyWaves.Count > 0)
        {
            StartNextWave(); // start the first wave
        }
        else
        {
            Debug.LogError("[WaveManager] No enemy wave data loaded or DataManager not found. Check DataManager setup and Script Execution Order.");
        }
    }

    // Call this method to advance to the next wave
    public void StartNextWave()
    {
        currentWaveIndex++; // move to the next wave

        if (currentWaveIndex < DataManager.Instance.allEnemyWaves.Count)
        {
            EnemyWave currentWave = DataManager.Instance.allEnemyWaves[currentWaveIndex];
            Debug.Log($"[WaveManager] Starting Wave: {currentWave.waveName} (ID: {currentWave.waveId})");

            // Stop any existing wave coroutine to prevent overlap, e.g., if a new wave starts early
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
            }

            // Reset enemiesRemainingInWave for the new wave
            enemiesRemainingInWave = 0;

            // Determine how the wave should be spawned based on its condition
            switch (currentWave.spawnCondition)
            {
                case "time_based":
                    if (float.TryParse(currentWave.conditionValue, out float delay))
                    {
                        currentWaveCoroutine = StartCoroutine(SpawnWaveAfterDelay(currentWave, delay));
                    }
                    else
                    {
                        Debug.LogError($"[WaveManager] Invalid conditionValue for time_based wave: {currentWave.conditionValue} in wave {currentWave.waveId}. Expected a float value.");
                    }
                    break;
                case "initial_spawn": // For waves that spawn all at once at the start
                    SpawnEnemiesInWave(currentWave);
                    break;
                default:
                    Debug.LogWarning($"[WaveManager] Unknown spawnCondition: {currentWave.spawnCondition} for wave {currentWave.waveId}. Defaulting to initial_spawn.");
                    SpawnEnemiesInWave(currentWave); // Fallback to immediate spawn
                    break;
            }
        }
        else
        {
            Debug.Log("[WaveManager] All waves completed! No more waves to spawn.");
            // Implement game over or victory condition here, e.g.:
            // GameUIManager.Instance.ShowVictoryScreen();
        }
    }

    private IEnumerator SpawnWaveAfterDelay(EnemyWave wave, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemiesInWave(wave);
    }

    private void SpawnEnemiesInWave(EnemyWave wave)
    {
        int totalEnemiesExpected = 0;
        if (enemyPrefab == null)
        {
            Debug.LogError("[WaveManager] Enemy Prefab is not assigned in the Inspector! Cannot spawn enemies.");
            return;
        }

        foreach (EnemyToSpawn entry in wave.enemiesToSpawn)
        {
            // Request EnemyData from DataManager
            // This assumes EnemyData is the plain C# class without MonoBehaviour
            EnemyData enemyData = DataManager.Instance.GetEnemyData(entry.enemyId);

            if (enemyData != null)
            {
                Transform spawnPoint = GetSpawnPoint(entry.spawnPointTag);

                if (spawnPoint != null)
                {
                    for (int i = 0; i < entry.count; i++)
                    {
                        // Instantiate the generic enemy prefab at the spawn point's position
                        GameObject newEnemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                        // Get the EnemyController component from the instantiated enemy.
                        // Ensure EnemyController.cs is attached to your enemyPrefab.
                        EnemyController enemyController = newEnemyGO.GetComponent<EnemyController>();
                        if (enemyController != null)
                        {
                            // Initialize the enemy with its specific loaded data
                            enemyController.Initialize(enemyData);
                            enemiesRemainingInWave++; // Increment here for each spawned enemy
                        }
                        else
                        {
                            Debug.LogError($"[WaveManager] EnemyPrefab '{enemyPrefab.name}' is missing an EnemyController component! Cannot initialize enemy.", enemyPrefab);
                            Destroy(newEnemyGO); // Clean up the uninitialized enemy
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[WaveManager] No suitable spawn point found for tag '{entry.spawnPointTag}' for enemy {enemyData.name}. Skipping this spawn entry.", this);
                }
            }
            else
            {
                Debug.LogWarning($"[WaveManager] Enemy data for ID '{entry.enemyId}' not found in DataManager. Skipping this spawn entry.", this);
            }
        }
        Debug.Log($"[WaveManager] All enemies for Wave {wave.waveName} initiated spawning. Total expected enemies for this wave: {enemiesRemainingInWave}");
        // If no enemies were spawned for some reason, immediately check for wave completion
        if (enemiesRemainingInWave == 0)
        {
            Debug.Log("[WaveManager] No enemies were spawned for this wave. Checking for next wave.");
            StartNextWave();
        }
    }

    // This method finds a suitable spawn point based on the provided tag.
    private Transform GetSpawnPoint(string tag)
    {
        // Debug.Log is often useful, but can be verbose. Keep it during debugging, remove for final build.
        // Debug.Log($"[WaveManager Debug] Attempting to find spawn point with tag: '{tag}'");

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager Debug] 'spawnPoints' array is null or empty in the Inspector! Please assign spawn point GameObjects.", this);
            return null;
        }

        // Collect all valid points that match the tag
        List<Transform> taggedPoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (point != null && point.CompareTag(tag))
            {
                taggedPoints.Add(point);
            }
        }

        if (taggedPoints.Count > 0)
        {
            // If the tag is "random", pick one randomly from the matching tagged points
            if (tag.Equals("random", StringComparison.OrdinalIgnoreCase)) // Use case-insensitive comparison for "random"
            {
                Transform randomPoint = taggedPoints[Random.Range(0, taggedPoints.Count)];
                Debug.Log($"[WaveManager Debug] Found random spawn point for tag '{tag}': {randomPoint.name}");
                return randomPoint;
            }
            else
            {
                // For specific tags, return the first one found (or extend logic if multiple points can share a specific tag)
                Debug.Log($"[WaveManager Debug] Found specific spawn point for tag '{tag}': {taggedPoints[0].name}");
                return taggedPoints[0];
            }
        }
        // Fallback for "random" tag: if no specific "random" tagged points were found, check ALL points
        // This provides a safety net, but it's better to ensure your random spawn points are tagged correctly.
        else if (tag.Equals("random", StringComparison.OrdinalIgnoreCase))
        {
            List<Transform> allValidPoints = new List<Transform>();
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    allValidPoints.Add(point);
                }
            }

            if (allValidPoints.Count > 0)
            {
                Transform randomPoint = allValidPoints[Random.Range(0, allValidPoints.Count)];
                Debug.LogWarning($"[WaveManager Debug] No specific 'random' tagged points found. Using a random available spawn point: {randomPoint.name}");
                return randomPoint;
            }
        }

        // If no spawn point was found after all checks
        Debug.LogWarning($"[WaveManager Debug] No spawn point found for tag: '{tag}'. Please ensure GameObjects are assigned to 'Spawn Points' array and have the correct tag in the Inspector.", this);
        return null;
    }

    // Call this method from EnemyController when an enemy is defeated
    public void OnEnemyDefeated()
    {
        enemiesRemainingInWave--;
        Debug.Log($"[WaveManager] Enemy defeated. Enemies remaining in wave: {enemiesRemainingInWave}");
        if (enemiesRemainingInWave <= 0)
        {
            Debug.Log("[WaveManager] Wave cleared! Starting next wave soon...");
            // You can add a delay here before starting the next wave, e.g., using a coroutine:
            // StartCoroutine(StartNextWaveAfterDelay(3f)); // 3-second delay
            StartNextWave();
        }
    }

    /*
    // Example of adding a delay before starting the next wave
    private IEnumerator StartNextWaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }
    */
}