using System;
using UnityEngine;
[System.Serializable]

public class EnemyToSpawn : MonoBehaviour
{
    public string enemyId;
    public int count;
    public string spawnPointTag; // tag to identify a specific spawn point (e.g., "TopSpawn", "LeftSpawn")
}