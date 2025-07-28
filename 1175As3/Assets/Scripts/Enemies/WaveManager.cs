using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    // Singleton pattern for easy access from other scripts
    public static WaveManager Instance { get; private set; }

    [Tooltip("Assign your generic enemy prefab here. This prefab must have an EnemyController component.")]
    public GameObject enemyPrefab;

    [Tooltip("Assign GameObjects here that will serve as spawn points for enemies. Ensure they have relevant tags (e.g., 'top', 'bottom', 'random').")]
    public Transform[] spawnPoints;

    // Internal state variables for wave management
    private int currentWaveIndex = -1; // Starts at -1, so the first call to StartNextWave increments it to 0.
    private Coroutine currentWaveSpawnCoroutine; // Corrected variable name: 'currentWaveSpawnCoroutine'
    private int enemiesRemainingInWave = 0; // Tracks enemies currently active from the *last spawned* wave.

    // This variable stores the absolute game time (Time.time) when the *next* wave should begin.
    private float nextWaveScheduledTime = 0f;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate instances if one already exists
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ensure DataManager has loaded data before trying to access it
        // This check is important due to script execution order
        if (DataManager.Instance == null)
        {
            Debug.LogError("[WaveManager] DataManager.Instance is null! Please ensure DataManager script is in the scene and initializes before WaveManager (check Script Execution Order).");
            enabled = false; // Disable this component to prevent further errors
            return;
        }

        if (DataManager.Instance.allEnemyWaves == null || DataManager.Instance.allEnemyWaves.Count == 0)
        {
            Debug.LogError("[WaveManager] No enemy wave data loaded from DataManager. Please check your 'enemy_waves.json' file and DataManager setup.");
            enabled = false; // Disable this component
            return;
        }

        Debug.Log($"[WaveManager] Loaded {DataManager.Instance.allEnemyWaves.Count} waves from DataManager.");

        // Initialize the 'nextWaveScheduledTime' for the very first wave (Wave 0).
        // This ensures the first wave starts at its specified 'conditionValue' time, not immediately.
        if (DataManager.Instance.allEnemyWaves.Count > 0)
        {
            EnemyWave firstWave = DataManager.Instance.allEnemyWaves[0];
            float parsedConditionValue;

            // Use TryParse for robust error handling in case 'conditionValue' is not a valid number.
            if (float.TryParse(firstWave.conditionValue, out parsedConditionValue))
            {
                nextWaveScheduledTime = parsedConditionValue;
                Debug.Log($"[WaveManager] Initialized. First wave '{firstWave.waveName}' scheduled for {nextWaveScheduledTime:F2} seconds game time.");
            }
            else
            {
                // Fallback if parsing fails: schedule for immediate start and log an error.
                Debug.LogError($"[WaveManager] ERROR: Failed to parse conditionValue for first wave '{firstWave.waveName}' ('{firstWave.conditionValue}'). Expected a float value. Setting nextWaveScheduledTime to 0 (will start immediately).");
                nextWaveScheduledTime = 0f;
            }
        }
        else
        {
            Debug.LogWarning("[WaveManager] No waves defined in enemy_waves.json. Game will not spawn enemies.");
            enabled = false; // Disable if no waves to process
        }
    }

    public void ResetWaveManager()
    {
        Debug.Log("[WaveManager] Resetting wave manager state.");

        // Stop any active wave spawning coroutine to prevent enemies from continuing to spawn from old waves
        if (currentWaveSpawnCoroutine != null)
        {
            StopCoroutine(currentWaveSpawnCoroutine);
            currentWaveSpawnCoroutine = null;
        }

        currentWaveIndex = -1; // Reset to before the first wave
        enemiesRemainingInWave = 0; // Reset enemy count
        nextWaveScheduledTime = 0f; // Reset to allow the first wave to schedule based on its conditionValue

        // Optional: Destroy all remaining enemies from previous waves if any are left in the scene
        // This assumes your enemy GameObjects have a specific tag, e.g., "Enemy".
        // Make sure your enemy prefabs are tagged "Enemy" (or whatever tag you use).
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
        }

        // Re-initialize for the first wave schedule (similar to Start() logic)
        // This ensures the wave manager is ready to schedule the very first wave again.
        if (DataManager.Instance != null && DataManager.Instance.allEnemyWaves != null && DataManager.Instance.allEnemyWaves.Count > 0)
        {
            EnemyWave firstWave = DataManager.Instance.allEnemyWaves[0];
            float parsedConditionValue;
            if (float.TryParse(firstWave.conditionValue, out parsedConditionValue))
            {
                nextWaveScheduledTime = parsedConditionValue;
                Debug.Log($"[WaveManager] Reset complete. First wave '{firstWave.waveName}' re-scheduled for {nextWaveScheduledTime:F2} seconds game time.");
            }
            else
            {
                Debug.LogError($"[WaveManager] ERROR during reset: Failed to parse conditionValue for first wave '{firstWave.waveName}'. Setting nextWaveScheduledTime to 0f.");
                nextWaveScheduledTime = 0f;
            }
        }
        else
        {
            Debug.LogWarning("[WaveManager] No wave data available after reset. Waves may not start.");
            enabled = false; // Disable if no waves to process
        }

        // Re-enable the script if it was disabled (e.g., if no waves were found initially)
        enabled = true;
    }

    void Update()
    {
        // Ensure DataManager is ready and there are waves to process.
        // This check prevents NullReferenceExceptions if DataManager fails to load.
        if (DataManager.Instance == null || DataManager.Instance.allEnemyWaves == null || DataManager.Instance.allEnemyWaves.Count == 0)
        {
            return; // No data, nothing to do.
        }

        // --- Core Logic for Time-Based Wave Progression ---
        // This checks if it's time to trigger the next wave based on 'nextWaveScheduledTime'.
        // currentWaveIndex + 1 represents the *next* wave we are about to process.
        if ((currentWaveIndex + 1) < DataManager.Instance.allEnemyWaves.Count && Time.time >= nextWaveScheduledTime)
        {
            StartNextWave(); // Trigger the process for the next wave in sequence.
        }
        // Special handling for the very first wave (when currentWaveIndex is -1).
        // This ensures the initial wave is triggered correctly based on its scheduled time.
        else if (currentWaveIndex == -1 && DataManager.Instance.allEnemyWaves.Count > 0 && Time.time >= nextWaveScheduledTime)
        {
            StartNextWave(); // Start the first wave if its scheduled time is met.
        }
    }

    // This method advances to the next wave, initiates its spawning, and schedules the subsequent wave.
    public void StartNextWave()
    {
        currentWaveIndex++; // Increment to point to the next wave to be processed (e.g., -1 becomes 0, 0 becomes 1).

        // Check if there are still waves left in the list to process.
        if (currentWaveIndex < DataManager.Instance.allEnemyWaves.Count)
        {
            EnemyWave currentWave = DataManager.Instance.allEnemyWaves[currentWaveIndex];
            Debug.Log($"[WaveManager] Starting Wave: {currentWave.waveName} (ID: {currentWave.waveId}) at game time {Time.time:F2}s.");

            // Stop any existing wave spawning coroutine to prevent overlap or unexpected behavior.
            if (currentWaveSpawnCoroutine != null) // Corrected variable name
            {
                StopCoroutine(currentWaveSpawnCoroutine); // Corrected variable name
            }

            enemiesRemainingInWave = 0; // Reset the count of enemies for the new wave.

            // Determine how the current wave's enemies should be spawned based on its 'spawnCondition'.
            switch (currentWave.spawnCondition)
            {
                case "time_based":
                    // For 'time_based' waves, the main delay is handled by the Update() loop.
                    // This coroutine just initiates the spawning on the next frame (or after a very short internal delay).
                    currentWaveSpawnCoroutine = StartCoroutine(SpawnWaveImmediately(currentWave)); // Corrected variable name
                    break;
                case "initial_spawn": // For waves that spawn all at once at the start of their trigger.
                    SpawnEnemiesInWave(currentWave);
                    break;
                // Add more spawn conditions here if your game implements them (e.g., "trigger_area", "all_enemies_defeated").
                default:
                    // Log a warning if an unknown spawnCondition is encountered and default to immediate spawn.
                    Debug.LogWarning($"[WaveManager] Unknown spawnCondition: '{currentWave.spawnCondition}' for wave '{currentWave.waveId}'. Defaulting to 'initial_spawn'.");
                    SpawnEnemiesInWave(currentWave);
                    break;
            }

            // --- Schedule the *next* wave's start time ---
            // This is crucial for the Update() loop to know when to trigger the subsequent wave.
            if ((currentWaveIndex + 1) < DataManager.Instance.allEnemyWaves.Count)
            {
                EnemyWave nextWave = DataManager.Instance.allEnemyWaves[currentWaveIndex + 1];
                float parsedConditionValue;

                // Robustly parse the 'conditionValue' for the next wave.
                if (float.TryParse(nextWave.conditionValue, out parsedConditionValue))
                {
                    nextWaveScheduledTime = parsedConditionValue;
                    Debug.Log($"[WaveManager] Next wave '{nextWave.waveName}' scheduled for {nextWaveScheduledTime:F2} seconds game time.");
                }
                else
                {
                    // Fallback if parsing fails: schedule for a short delay from current time to prevent freezing.
                    Debug.LogError($"[WaveManager] ERROR: Failed to parse conditionValue for next wave '{nextWave.waveName}' ('{nextWave.conditionValue}'). Please check enemy_waves.json. Setting nextWaveScheduledTime to current time + 5s fallback.");
                    nextWaveScheduledTime = Time.time + 5f;
                }
            }
            else
            {
                // If this was the last wave in the list, set 'nextWaveScheduledTime' to a very high value
                // to prevent Update() from trying to trigger non-existent waves.
                Debug.Log("[WaveManager] All defined waves have been scheduled. Waiting for final enemies to be cleared.");
                nextWaveScheduledTime = float.MaxValue; // Effectively stops further wave scheduling.
            }
        }
        else
        {
            // This block is reached when 'currentWaveIndex' goes beyond the last wave in the list.
            Debug.Log("[WaveManager] All waves completed! No more waves to spawn.");
            // --- Implement your game over or victory condition here ---
            // Example: GameUIManager.Instance.ShowVictoryScreen();
        }
    }

    // Coroutine to initiate enemy spawning for a wave.
    // The 'delay' parameter is mostly for internal wave pacing if needed (e.g., enemies trickling in).
    // The main delay between waves is handled by 'Update()' and 'nextWaveScheduledTime'.
    private IEnumerator SpawnWaveImmediately(EnemyWave wave)
    {
        // Yield for one frame to ensure this coroutine runs on the next frame,
        // which can sometimes help with initialization order.
        yield return null;
        SpawnEnemiesInWave(wave);
    }

    // Handles the actual instantiation and initialization of enemies for a given wave.
    private void SpawnEnemiesInWave(EnemyWave wave)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[WaveManager] Enemy Prefab is not assigned in the Inspector! Cannot spawn enemies.");
            return;
        }

        // Check if the wave actually has enemies defined to spawn.
        // Change: Used .Count instead of .Length for List<EnemyToSpawn>
        if (wave.enemiesToSpawn == null || wave.enemiesToSpawn.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] Wave '{wave.waveName}' (ID: {wave.waveId}) has no enemies defined in 'enemiesToSpawn'. No enemies will be spawned for this wave.");
            return; // Exit if no enemies to spawn.
        }

        // Iterate through each type of enemy defined for this wave.
        foreach (EnemyToSpawn entry in wave.enemiesToSpawn)
        {
            // Retrieve the specific EnemyData from DataManager using the enemyId.
            EnemyData enemyData = DataManager.Instance.GetEnemyData(entry.enemyId);

            if (enemyData != null)
            {
                // Get a suitable spawn point based on the tag defined in JSON.
                Transform spawnPoint = GetSpawnPoint(entry.spawnPointTag);

                if (spawnPoint != null)
                {
                    // --- Enemy Spawning with Offset ---
                    // Define a small offset to prevent enemies from spawning directly on top of each other.
                    // Adjust this value (e.g., 0.5f) based on your enemy sizes and desired spacing.
                    float individualEnemyOffset = 0.6f;
                    Vector3 baseSpawnPosition = spawnPoint.position;

                    for (int i = 0; i < entry.count; i++)
                    {
                        // Calculate an offset position for each enemy.
                        // Example: Spreading them out along the X-axis.
                        Vector3 currentSpawnPosition = baseSpawnPosition;
                        currentSpawnPosition.x += i * individualEnemyOffset;

                        // Alternative: Random spread around the spawn point (adjust multiplier for spread area)
                        // currentSpawnPosition += (Vector3)UnityEngine.Random.insideUnitCircle * individualEnemyOffset * i;

                        // Instantiate the generic enemy prefab at the calculated position.
                        GameObject newEnemyGO = Instantiate(enemyPrefab, currentSpawnPosition, Quaternion.identity);

                        // Get the EnemyController component from the newly instantiated enemy.
                        EnemyController enemyController = newEnemyGO.GetComponent<EnemyController>();
                        if (enemyController != null)
                        {
                            // Initialize the enemy with its specific loaded data.
                            enemyController.Initialize(enemyData);
                            enemiesRemainingInWave++; // Increment the total count of active enemies.
                        }
                        else
                        {
                            Debug.LogError($"[WaveManager] EnemyPrefab '{enemyPrefab.name}' is missing an EnemyController component! Cannot initialize enemy. Destroying uninitialized enemy.", enemyPrefab);
                            Destroy(newEnemyGO); // Clean up if initialization fails.
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[WaveManager] No suitable spawn point found for tag '{entry.spawnPointTag}' for enemy '{enemyData.name}'. Skipping this spawn entry.", this);
                }
            }
            else
            {
                Debug.LogWarning($"[WaveManager] Enemy data for ID '{entry.enemyId}' not found in DataManager. Skipping this spawn entry.", this);
            }
        }
        Debug.Log($"[WaveManager] All enemies for Wave '{wave.waveName}' initiated spawning. Total enemies to defeat for this wave: {enemiesRemainingInWave}");

        // If, after attempting to spawn all enemies, 'enemiesRemainingInWave' is still 0,
        // it means no enemies were successfully spawned for this wave (e.g., due to errors).
        if (enemiesRemainingInWave == 0)
        {
            Debug.LogWarning($"[WaveManager] Wave '{wave.waveName}' spawned 0 enemies. This might be due to missing enemy data, prefab, or spawn points. This wave is effectively 'cleared' for progression.");
            // No direct call to StartNextWave() here; the Update() method will handle progression based on time.
        }
    }

    // Helper method to find a suitable spawn point based on its tag.
    private Transform GetSpawnPoint(string tag)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] 'spawnPoints' array is null or empty in the Inspector! Please assign spawn point GameObjects.", this);
            return null;
        }

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
            // If the tag is "random", pick one randomly from the matching tagged points.
            if (tag.Equals("random", StringComparison.OrdinalIgnoreCase))
            {
                return taggedPoints[UnityEngine.Random.Range(0, taggedPoints.Count)];
            }
            else
            {
                // For specific tags, return the first one found.
                return taggedPoints[0];
            }
        }
        // Fallback for "random" tag: if no specific "random" tagged points were found,
        // try to pick a random point from *all* assigned spawn points.
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
                Debug.LogWarning($"[WaveManager] No specific 'random' tagged points found. Using a random available spawn point from all assigned points.");
                return allValidPoints[UnityEngine.Random.Range(0, allValidPoints.Count)];
            }
        }

        Debug.LogWarning($"[WaveManager] No spawn point found for tag: '{tag}'. Please ensure GameObjects are assigned to 'Spawn Points' array and have the correct tag in the Inspector.", this);
        return null;
    }

    // This method is called by EnemyController when an enemy is defeated.
    public void OnEnemyDefeated()
    {
        // Integration with DynamicDataManager (assuming it exists and works).
        if (DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.IncrementEnemiesDefeated();
        }
        else
        {
            Debug.LogError("DynamicDataManager not found! Cannot increment enemies defeated. (If you don't have this, remove this block)");
        }

        // --- FIX: Only decrement if the count is greater than zero ---
        if (enemiesRemainingInWave > 0)
        {
            enemiesRemainingInWave--; // Decrement the count of active enemies.
        }
        else
        {
            // Log a warning if this happens, as it indicates a potential issue
            Debug.LogWarning("[WaveManager] OnEnemyDefeated called but enemiesRemainingInWave is already zero or negative. This might indicate an issue with enemy counting or duplicate calls.", this);
        }

        Debug.Log($"[WaveManager] Enemy defeated. Enemies remaining in current wave: {enemiesRemainingInWave}");

        // This block checks if all enemies from the currently spawned wave have been defeated.
        // For time-based waves, this doesn't trigger the next wave, but can be used for UI updates or scoring.
        if (enemiesRemainingInWave <= 0)
        {
            Debug.Log("[WaveManager] All enemies from the current wave have been cleared!");

            // Increment waves completed in DynamicDataManager (if applicable for your game's scoring).
            if (DynamicDataManager.Instance != null)
            {
                DynamicDataManager.Instance.IncrementWavesCompleted();
            }
            else
            {
                Debug.LogError("DynamicDataManager not found! Cannot increment waves completed. (If you don't have this, remove this block)");
            }

            // If this was the last wave defined in your JSON, and all its enemies are cleared,
            // then the game might be "completed".
            if ((currentWaveIndex + 1) >= DataManager.Instance.allEnemyWaves.Count)
            {
                Debug.Log("[WaveManager] All enemies from all *scheduled* waves have been defeated! Game Over / Victory!");
            }
        }
    }

    /*
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
        // to update dynamic data -- shumin
        // --- DynamicDataManager Integration: Increment Enemies Defeated
        if (DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.IncrementEnemiesDefeated();
            Debug.Log($"[DynamicDataManager] Enemies Defeated: {DynamicDataManager.Instance.GetEnemiesDefeated()}"); // Log current count -- can be removed afterwards
        }
        else
        {
            //can be removed afterwards just to check :)
            Debug.LogError("DynamicDataManager not found! Cannot increment enemies defeated.");
        }

        enemiesRemainingInWave--;
        Debug.Log($"[WaveManager] Enemy defeated. Enemies remaining in wave: {enemiesRemainingInWave}");
        if (enemiesRemainingInWave <= 0)
        {
            Debug.Log("[WaveManager] Wave cleared! Starting next wave soon...");

            // to update dynamic data -- shumin
            // --- DynamicDataManager Integration: Increment Waves Defeated
            if(DynamicDataManager.Instance != null)
            {
                DynamicDataManager.Instance.IncrementWavesCompleted();
                Debug.Log($"[DynamicDataManager] Waves Completed: {DynamicDataManager.Instance.GetWavesCompleted()}"); // Log current count -- same for this
            }
            else
            {
                Debug.LogError("DynamicDataManager not found! Cannot increment waves completed."); // this too :)
            }

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

