using System;
using UnityEngine;
[System.Serializable]

public class Enemy : MonoBehaviour
{
    public string id;
    public string name;
    public int health;
    public float movementSpeed;
    public int contactDamage;
    public float bulletFiringRate;
    public string movementPattern;
    public string behaviour;
    public string spritePath; //to load the sprite dynamically
}