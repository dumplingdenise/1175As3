using System;
using UnityEngine;
[Serializable]

public class Enemy : MonoBehaviour
{
    public string id;
    public string name;
    public float health;
    public float movementSpeed;
    public float contactDamage;
    public float bulletFiringRate;
    public string movementPattern;
    public string behaviour;
    public string spritePath; //to load the sprite dynamically
}
