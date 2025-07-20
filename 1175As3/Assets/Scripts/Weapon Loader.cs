using System;
using UnityEngine;

public class WeaponLoader : MonoBehaviour
{
    public TextAsset weaponCSVFile;
    public Sprite[] weaponSprites;

    public WeaponList weaponList = new WeaponList();

    void Start()
    {
        ReadCSV();
    }

    void ReadCSV()
    {
        string[] data = weaponCSVFile.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (data == null || data.Length <= 1)
        {
            Debug.LogError("Weapon CSV file is missing or empty");
            return;
        }

        weaponList.weapons = new WeaponData[data.Length - 1];

        for (int i = 1; i < data.Length; i++)
        {
            string[] parts = data[i].Trim().Split(',');

            if (parts.Length < 7) continue;

            WeaponData weapon = new WeaponData
            {
                id = parts[0],
                weaponName = parts[1],
                description = parts[2],
                damage = float.Parse(parts[3]),
                fireRate = float.Parse(parts[4]),
                bulletsPerShot = int.Parse(parts[5]),
                spreadAngle = float.Parse(parts[6]),
                spriteName = parts[7]
            };

            foreach (var sprite in weaponSprites)
            {
                if (sprite.name == weapon.spriteName)
                {
                    weapon.weaponSprite = sprite;
                    break;
                }
            }

            weaponList.weapons[i - 1] = weapon;
        }
    }
}
