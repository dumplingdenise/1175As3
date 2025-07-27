using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class EnemyWave : MonoBehaviour
{
    public string waveId;
    public string waveName;
    public string spawnCondition; // e.g., "time_based", "trigger_area", "all_enemies_defeated"
    public string conditionValue; // e.g., "30" for 30 seconds, or "Area1" for a trigger
    public List<EnemyToSpawn> enemiesToSpawn;
}