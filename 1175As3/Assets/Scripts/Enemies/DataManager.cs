using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // make sure installed

public class DataManager : MonoBehaviour
{
    // Singleton pattern for easy access from other scripts
    public static DataManager Instance { get; private set; }

    public List<Enemy> allEnemies = new List<Enemy>();
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
            allEnemies = JsonConvert.DeserializeObject<List<Enemy>>(jsonContent);
            Debug.Log($"[DataManager] Loaded {allEnemies.Count} enemies from {fileName}.");
        }
        else
        {
            Debug.LogError($"[DataManager] Enemy data file not found at: {fullPath}");
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
    public Enemy GetEnemyData(string enemyId)
    {
        return allEnemies.Find(enemy => enemy.id == enemyId);
    }

    // You can add similar helper methods for other data types (e.g., GetCharacterData)
}