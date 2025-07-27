using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

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
        // FOR TESTING ONLY: Press 'S' to save game stats and see the debug log
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveGameStats();
            Debug.Log("Attempting to save game stats via 'p' key press."); // Add this for more clarity
        }
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

    private AllGameStats ReadGameHistoryFromFile()
    {
        //define the file pah
        string filePath = Application.persistentDataPath + "/game_stats.json";

        //if file existm read the content
        if (File.Exists(filePath))
        {
            // read entire json content from the file
            string json = File.ReadAllText(filePath);

            //convert json string back to AllGameStas object
            AllGameStats loadedAllStats = JsonUtility.FromJson<AllGameStats>(json);

            //return loaded AllGameStats object
            return loadedAllStats;
        }
        else
        {
            // Initialize the AllGameStats container with an empty list, ready for new entries
            AllGameStats newGameHistory = new AllGameStats { statsEntries = new List<GameStats>() };

            return newGameHistory;
        }
    }

    //save game stats to an external file
    public void SaveGameStats()
    {
        Debug.Log("SaveGameStats method entered."); // Add this line

        AllGameStats allStats = ReadGameHistoryFromFile();

        //create instance of GameStats to hold the data
        GameStats saveStats = new GameStats();

        saveStats.timestamp = System.DateTime.Now.ToString();

        //Populate the GameStats object with current manager's data
        saveStats.enemiesDefeated = enemiesdefeated;
        saveStats.wavescompleted = wavescompleted;
        saveStats.totaldistancetraveled = totaldistancetraveled;

        //add current stats to the historical list
        allStats.statsEntries.Add(saveStats);

        //convert whole list to JSON
        string json = JsonUtility.ToJson(allStats, true);

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

            //Convert the JSON into an AllGameStats object, as the file now stores history
            AllGameStats loadedAllStatsHistory = JsonUtility.FromJson<AllGameStats>(json);

            // Check if there are any game stats entries in the loaded history.
            if (loadedAllStatsHistory != null && loadedAllStatsHistory.statsEntries.Count > 0)
            {
                // Get the last (most recent) GameStats entry from the history.
                GameStats latestStats = loadedAllStatsHistory.statsEntries[loadedAllStatsHistory.statsEntries.Count - 1];

                // Update the DynamicDataManager's variables with the loaded data.
                enemiesdefeated = latestStats.enemiesDefeated;
                wavescompleted = latestStats.wavescompleted;
                totaldistancetraveled = latestStats.totaldistancetraveled;
            }
            else
            {
                Debug.LogWarning("Save file found but no game stats entires were present");

                // reset current stats to 0.
                enemiesdefeated = 0;
                wavescompleted = 0;
                totaldistancetraveled = 0;
            }

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

    // Timestamp for when this specific game session's data was recorded.
    public string timestamp;

}

[System.Serializable]
public class AllGameStats
{
    public List<GameStats> statsEntries;

}