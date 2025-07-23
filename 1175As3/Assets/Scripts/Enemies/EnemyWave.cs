using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]

public class EnemyWave : MonoBehaviour
{
    public string waveId; // Unique identifier for this wave
    public string waveName; // Display name of the wave
    public string spawnCondition; // e.g., "time_based", "trigger_area", "all_enemies_defeated"
    public string conditionValue; // Value associated with the condition (e.g., "30" for 30 seconds, or "Area1" for a trigger)
    public List<EnemyToSpawn> enemiesToSpawn; // List of enemies and their quantities for this wave
}