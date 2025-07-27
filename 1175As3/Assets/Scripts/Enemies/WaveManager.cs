using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    
    // singleton pattern
    public static WaveManager Instance { get; private set; }

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
            // no DontDestroyOnLoad here if you want it to reset per scene.
            // if want it to persist, add DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ensure DataManager has loaded data before trying to access it
        if (DataManager.Instance != null && DataManager.Instance.allEnemyWaves.Count > 0)
        {
            StartNextWave(); // start the first wave
        }
        else
        {
            Debug.LogError("[WaveManager] No enemy wave data loaded or DataManager not found. Check DataManager setup.");
        }
    }

    // call this method to advance to the next wave
    public void StartNextWave()
    {
        currentWaveIndex++; // move to the next wave

        if (currentWaveIndex < DataManager.Instance.allEnemyWaves.Count)
        {
            EnemyWave currentWave = DataManager.Instance.allEnemyWaves[currentWaveIndex];
            Debug.Log($"[WaveManager] Starting Wave: {currentWave.waveName} (ID: {currentWave.waveId})");

            // stop any existing wave coroutine to prevent overlap
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
            }

            // determine how the wave should be spawned based on its condition
            switch (currentWave.spawnCondition)
            {
                case "time_based":
                    if (float.TryParse(currentWave.conditionValue, out float delay))
                    {

                        currentWaveCoroutine = StartCoroutine(SpawnWaveAfterDelay(currentWave, delay));
                    }
                    else
                    {
                        Debug.LogError($"[WaveManager] Invalid time_based condition value for wave {currentWave.waveId}: {currentWave.conditionValue}");
                        currentWaveCoroutine = StartCoroutine(SpawnEnemiesInWave(currentWave)); // spawn immediately if value invalid
                    }
                    break;
                case "trigger_area":
                    // this wave requires an external trigger to activate (e.g., player enters a zone)
                    // need a separate Trigger script that calls WaveManager.Instance.ActivateTriggeredWave(currentWave.waveId);
                    Debug.Log($"[WaveManager] Wave {currentWave.waveName} awaiting trigger: {currentWave.conditionValue}");
                    break;
                case "all_enemies_defeated":
                    // this implies the previous wave must be fully cleared.
                    // if this is the *first* wave, it will just start.
                    // if it's a subsequent wave, it needs to be triggered after the previous wave's enemies are all gone.
                    currentWaveCoroutine = StartCoroutine(SpawnEnemiesInWave(currentWave));
                    break;
                default:
                    Debug.LogWarning($"[WaveManager] Unknown spawn condition for wave {currentWave.waveId}: {currentWave.spawnCondition}. Spawning immediately.");
                    currentWaveCoroutine = StartCoroutine(SpawnEnemiesInWave(currentWave));
                    break;
            }
        }
        else
        {
            Debug.Log("[WaveManager] All waves completed! No more waves to start.");
            // game end or level completion logic here
        }
    }

    // coroutine to handle time-based wave spawning
    private IEnumerator SpawnWaveAfterDelay(EnemyWave wave, float delay)
    {
        yield return new WaitForSeconds(delay); // wait for the specified delay
        currentWaveCoroutine = StartCoroutine(SpawnEnemiesInWave(wave)); // then start spawning enemies
    }

    // coroutine to spawn enemies for the current wave
    private IEnumerator SpawnEnemiesInWave(EnemyWave wave)
    {
        enemiesRemainingInWave = 0; // reset counter for the new wave

        foreach (EnemyToSpawn entry in wave.enemiesToSpawn)
        {
            EnemyData enemyData = DataManager.Instance.GetEnemyData(entry.enemyId);
            if (enemyData == null)
            {
                Debug.LogError($"[WaveManager] Enemy data for ID '{entry.enemyId}' not found! Skipping.");
                continue;
            }

            for (int i = 0; i < entry.count; i++)
            {
                Transform spawnPoint = GetSpawnPoint(entry.spawnPointTag);
                if (spawnPoint != null)
                {
                    // create a new GameObject for the enemy
                    GameObject enemyGO = new GameObject($"Enemy_{enemyData.name}_{i}");
                    enemyGO.transform.position = spawnPoint.position;
                    enemyGO.transform.SetParent(this.transform); // parent to WaveManager for organization

                    // add the EnemyController script and initialize it with data
                    EnemyController enemyController = enemyGO.AddComponent<EnemyController>();
                    enemyController.Initialize(enemyData);

                    enemiesRemainingInWave++; // increment count of enemies for this wave
                }
                else
                {
                    Debug.LogWarning($"[WaveManager] No suitable spawn point found for tag '{entry.spawnPointTag}' for enemy {enemyData.name}.");
                }
                yield return new WaitForSeconds(0.2f); // small delay between individual enemy spawns
            }
        }
        Debug.Log($"[WaveManager] All enemies for Wave {wave.waveName} initiated spawning. Total expected enemies: {enemiesRemainingInWave}");
    }

    // helper to get a spawn point by tag or randomly
    private Transform GetSpawnPoint(string tag = "")
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] No spawn points assigned!");
            return null;
        }

        if (!string.IsNullOrEmpty(tag) && tag.ToLower() != "random")
        {
            // try to find a specific spawn point by name/tag (you'd need to set names in Inspector)
            foreach (Transform sp in spawnPoints)
            {
                if (sp.name.Equals(tag, System.StringComparison.OrdinalIgnoreCase))
                {
                    return sp;
                }
            }
            Debug.LogWarning($"[WaveManager] Spawn point with tag '{tag}' not found. Using random spawn point instead.");
        }

        // Default to random if tag is empty, "random", or not found
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    // this method is called by EnemyController when an enemy is defeated
    public void OnEnemyDefeated()
    {
        enemiesRemainingInWave--;
        Debug.Log($"[WaveManager] Enemy defeated. Enemies remaining in wave: {enemiesRemainingInWave}");

        if (enemiesRemainingInWave <= 0)
        {
            Debug.Log("[WaveManager] All enemies in current wave defeated!");
            // after short delay, start the next wave (gives player a breather)
            StartCoroutine(DelayBeforeNextWave(3f)); // 3-second delay
        }
    }

    private IEnumerator DelayBeforeNextWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    // if implement "trigger_area" waves, you'd call this from a Trigger component
    public void ActivateTriggeredWave(string waveIdToActivate)
    {
        // Find the wave by its ID
        EnemyWave waveToActivate = DataManager.Instance.allEnemyWaves.Find(wave => wave.waveId == waveIdToActivate);
        if (waveToActivate != null)
        {
            // Ensure this is the *current* expected wave to avoid skipping
            if (DataManager.Instance.allEnemyWaves[currentWaveIndex]?.waveId == waveIdToActivate)
            {
                Debug.Log($"[WaveManager] Triggered wave '{waveIdToActivate}' activated.");
                currentWaveCoroutine = StartCoroutine(SpawnEnemiesInWave(waveToActivate));
            }
            else
            {
                Debug.LogWarning($"[WaveManager] Triggered wave '{waveIdToActivate}' activated, but it's not the current expected wave. Skipping.");
            }
        }
        else
        {
            Debug.LogError($"[WaveManager] Attempted to activate non-existent wave ID: {waveIdToActivate}");
        }
    }
}
