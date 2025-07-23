using System;
using UnityEngine;

public class WeaponLoader : MonoBehaviour
{
    public TextAsset weaponCSVFile;         // CSV file to read weapon data
    public Sprite[] weaponSprites;          // Sprites to match with weapon sprite names
    public WeaponSelection weaponSelection; // Reference to WeaponSelection (assign in Inspector)

    [System.Serializable]
    public class WeaponList
    {
        public WeaponData[] weapons;
    }

    public WeaponList weaponList = new WeaponList();

    void Start()
    {
        ReadCSV();

        if (weaponSelection != null)
        {
            weaponSelection.InitializeWeapons(weaponList.weapons);
        }
        else
        {
            Debug.LogWarning("WeaponSelection reference not assigned to WeaponLoader!");
        }
    }

    void ReadCSV()
    {
        if (weaponCSVFile == null)
        {
            Debug.LogError("CSV file not assigned!");
            return;
        }

        string[] data = weaponCSVFile.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (data.Length <= 1)
        {
            Debug.LogError("Weapon CSV file is missing or only contains header.");
            return;
        }

        weaponList.weapons = new WeaponData[data.Length - 1]; // skip header row

        for (int i = 1; i < data.Length; i++) // Start from 1 to skip header
        {
            string line = data[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split(',');

            if (parts.Length < 8)
            {
                Debug.LogWarning($"Skipping line {i} due to insufficient columns: {line}");
                continue;
            }

            WeaponData weapon = new WeaponData();
            weapon.id = parts[0];
            weapon.weaponName = parts[1];
            weapon.description = parts[2];
            weapon.damage = float.Parse(parts[3]);
            weapon.fireRate = float.Parse(parts[4]);
            weapon.bulletsPerShot = int.Parse(parts[5]);
            weapon.spreadAngle = float.Parse(parts[6]);
            weapon.spriteName = parts[7];

            // Match sprite name to assign sprite
            bool spriteFound = false;
            foreach (var sprite in weaponSprites)
            {
                if (sprite != null && sprite.name == weapon.spriteName)
                {
                    weapon.weaponSprite = sprite;
                    spriteFound = true;
                    break;
                }
            }

            if (!spriteFound)
            {
                Debug.LogWarning($"Sprite not found for weapon: {weapon.weaponName} (Sprite name: {weapon.spriteName})");
            }

            weaponList.weapons[i - 1] = weapon;
        }

        Debug.Log($"WeaponLoader: Loaded {weaponList.weapons.Length} weapons from CSV.");
    }
}
