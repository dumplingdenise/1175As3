using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string id;
    public string weaponName;
    public string description;
    public float damage;
    public float fireRate;
    public int bulletsPerShot;
    public float spreadAngle;
    public string spriteName;
    public Sprite weaponSprite;
}

[System.Serializable]
public class WeaponList
{
    public WeaponData[] weapons;
}
