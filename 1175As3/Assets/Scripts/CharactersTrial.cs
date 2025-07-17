using UnityEngine;

[System.Serializable]
public class CharactersTrial
{
    public string id;
    public string characterName;
    public string description;
    public float movementSpeed;
    public int maxHealth;
    /*public string initialWeaponId;*/
    public Sprite characterSprite;
}

[System.Serializable]
public class CharacterData
{
    public CharactersTrial[] characters;
}