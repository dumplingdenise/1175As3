using System;
using UnityEngine;
[Serializable]

public class EnemyToSpawn : MonoBehaviour
{
    public string enemyId; // The 'id' of the enemy to spawn, matching an entry in enemies.json
    public int count; // How many of this enemy type to spawn
    public string spawnPointTag; // Optional: Tag to identify a specific spawn point (e.g., "TopSpawn", "LeftSpawn")
}