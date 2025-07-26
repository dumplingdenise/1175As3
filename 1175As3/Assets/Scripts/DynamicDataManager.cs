using UnityEngine;
using System.IO;

public class DynamicDataManager : MonoBehaviour
{
    public static DynamicDataManager Instance;

    private int enemiesdefeated;
    private int wavescompleted;
    private float totaldistancetraveled;

    void Awake()
    {
        //check if an instance already exists
        if(Instance == null)
        {
            //set this instance as the singleton
            Instance = this;

            //make this gameobject persist across scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //if there is another instance that exists, destroy this instance
            Destroy(gameObject);
        }

        // Initialize the statistics
        enemiesdefeated = 0;
        wavescompleted = 0;
        totaldistancetraveled = 0;
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementEnemiesDefeated()
    {
        enemiesdefeated++;
    }

    public void IncrementWavesCompleted()
    {
        wavescompleted++;
    }

    //add the distance to the total distance traveled
    public void AddDistanceTraveled(float distance)
    {
        totaldistancetraveled += distance;
    }

    public int GetEnemiesDefeated()
    {
        return enemiesdefeated;
    }

    public int GetWavesCompleted()
    {
        return wavescompleted;
    }

    public float GetTotalDistanceTraveled()
    {
        return totaldistancetraveled;
    }

    //save game stats to an external file
    public void SaveGameStats()
    {
        //create instance of GameStats to hold the data
        GameStats saveStats = new GameStats();

        //Populate the GameStats object with current manager's data
        saveStats.enemiesDefeated = enemiesdefeated;
        saveStats.wavescompleted = wavescompleted;
        saveStats.totaldistancetraveled= totaldistancetraveled;

        // convert GameStats object into a Json formatted string
        string json = JsonUtility.ToJson(saveStats);

        // Define the file path where the JSON data will be saved.
        string filePath = Application.persistentDataPath + "/game_stats.json";

        // Write the JSON string to the specified file path
        File.WriteAllText(filePath, json);
        Debug.Log("Game states saved to: " + filePath);
    }

    //load GameStats
    public void LoadGameStats()
    {
        // Define the file path from where the JSON data will be loaded.
        string filePath = Application.persistentDataPath + "/game_stats.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            //specify the type of object to convert
            GameStats loadstats = JsonUtility.FromJson<GameStats>(json);

            // Update the DynamicDataManager's variables with the loaded data.
            enemiesdefeated = loadstats.enemiesDefeated;
            wavescompleted = loadstats.wavescompleted;
            totaldistancetraveled = loadstats.totaldistancetraveled;

            Debug.Log("Game stats loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + filePath);
        }
    }
}

// this class will hold the statistics that will be save to a file
[System.Serializable]
public class GameStats
{
    public int enemiesDefeated;
    public int wavescompleted;
    public float totaldistancetraveled;

}