using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // make sure installed

public class DataManager : MonoBehaviour
{
    // Singleton pattern for easy access from other scripts
    public static DataManager Instance { get; private set; }

    public List<EnemyData> allEnemies = new List<EnemyData>();
    public List<EnemyWave> allEnemyWaves = new List<EnemyWave>();
    // Add other data lists here (e.g., allCharacters, allWeapons)

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        LoadAllGameData();
    }

    private void LoadAllGameData()
    {
        // Load Enemy Data
        LoadEnemies("enemies.json"); // File name within StreamingAssets
        // Load Enemy Wave Data
        LoadEnemyWaves("enemy_waves.json"); // File name within StreamingAssets

        // Example: Load other data types
        // LoadCharacters("characters.json");
        // LoadWeapons("weapons.json");
    }

    private void LoadEnemies(string fileName)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(fullPath))
        {
            string jsonContent = File.ReadAllText(fullPath);
            // Deserialize directly into a List<EnemyData> using Newtonsoft.Json
            allEnemies = JsonConvert.DeserializeObject<List<EnemyData>>(jsonContent);
            Debug.Log($"[DataManager] Loaded {allEnemies.Count} enemies from {fileName}.");

            if (allEnemies != null && allEnemies.Count > 0)
            {
                Debug.Log($"[DataManager] Successfully deserialized {allEnemies.Count} enemies from {fileName}.");
                foreach (var enemy in allEnemies)
                {
                    Debug.Log($"[DataManager] Deserialized Enemy ID: '{enemy.id}', Name: '{enemy.name}', Health: {enemy.health}");
                }
            }
            else
            {
                Debug.LogWarning($"[DataManager] Deserialized {fileName} but the list of enemies is null or empty. Check Enemy C# class matches JSON structure.");
            }
        }
        else
        {
            Debug.LogError($"[DataManager] Enemy data file not found at: {fullPath}");
            allEnemies = new List<EnemyData>(); // initialize empty list to prevent further NREs
        }
    }

    private void LoadEnemyWaves(string fileName)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(fullPath))
        {
            string jsonContent = File.ReadAllText(fullPath);
            // Deserialize directly into a List<EnemyWaveData> using Newtonsoft.Json
            allEnemyWaves = JsonConvert.DeserializeObject<List<EnemyWave>>(jsonContent);
            Debug.Log($"[DataManager] Loaded {allEnemyWaves.Count} enemy waves from {fileName}.");
        }
        else
        {
            Debug.LogError($"[DataManager] Enemy wave data file not found at: {fullPath}");
        }
    }

    // Public helper method to retrieve EnemyData by ID
    public EnemyData GetEnemyData(string enemyId)
    {
        return allEnemies.Find(enemy => enemy.id == enemyId);
    }

    // You can add similar helper methods for other data types (e.g., GetCharacterData)
}