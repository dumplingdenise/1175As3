using UnityEngine;

[System.Serializable]
public class Characters
{
    public string id;
    public string name;
    public string description;
    public float movementSpeed;
    public int maxHealth;
    /*public string intialWeaponId;*/
    public Sprite characterSprite;
}

[System.Serializable]
public class CharacterData
{
    public Characters[] characters;
}