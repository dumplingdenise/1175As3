

// Assets/Scripts/Weapon Loader.cs
using UnityEngine;
using System.Collections.Generic; // Required for List

public class WeaponLoader : MonoBehaviour
{
    // Inspector-assigned fields
    public TextAsset weaponCSVFile;
    public Sprite[] weaponSprites;
    public WeaponSelection weaponSelectionUI; // Reference to the WeaponSelection script

    // Helper class for serialization (not directly used for loading, but for structure)
    [System.Serializable]
    public class WeaponList
    {
        public WeaponData[] weapons;
    }

    public WeaponList weaponList = new WeaponList(); // Holds the loaded weapon data

    void Start()
    {
        LoadWeaponsFromCSV();
    }

    void LoadWeaponsFromCSV()
    {
        // Check if the CSV file is assigned
        if (weaponCSVFile == null)
        {
            Debug.LogError("WeaponLoader: weaponCSVFile is NULL! Please assign the CSV TextAsset in the Inspector.");
            // Initialize WeaponSelection with an empty array if CSV is missing
            if (weaponSelectionUI != null)
            {
                weaponSelectionUI.InitializeWeapons(new WeaponData[0]);
            }
            return;
        }

        // Split the CSV text into lines, removing empty entries
        string[] data = weaponCSVFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Check if the CSV is empty or only contains a header
        if (data.Length <= 1)
        {
            Debug.LogError("WeaponLoader: Weapon CSV file is missing or contains only header. No weapon data to load.");
            if (weaponSelectionUI != null)
            {
                weaponSelectionUI.InitializeWeapons(new WeaponData[0]); // Pass empty array
            }
            return;
        }

        // Use a List to dynamically add weapons, then convert to array
        List<WeaponData> loadedWeapons = new List<WeaponData>();

        // Iterate through CSV lines, skipping the header (i = 1)
        for (int i = 1; i < data.Length; i++)
        {
            string line = data[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                Debug.LogWarning($"WeaponLoader: Skipping empty line at index {i}.");
                continue;
            }

            string[] parts = line.Split(',');

            // Ensure sufficient columns are present
            if (parts.Length < 8)
            {
                Debug.LogWarning($"WeaponLoader: Skipping line {i} due to insufficient data (expected 8 columns, got {parts.Length}): '{line}'");
                continue;
            }

            WeaponData weapon = new WeaponData();
            weapon.id = parts[0];
            weapon.weaponName = parts[1];
            weapon.description = parts[2];

            // Robust parsing with TryParse for numerical values
            if (!float.TryParse(parts[3], out weapon.damage)) { Debug.LogWarning($"WeaponLoader: Failed to parse damage for '{weapon.weaponName}' (value: '{parts[3]}'). Defaulting to 0."); weapon.damage = 0; }
            if (!float.TryParse(parts[4], out weapon.fireRate)) { Debug.LogWarning($"WeaponLoader: Failed to parse fireRate for '{weapon.weaponName}' (value: '{parts[4]}'). Defaulting to 0."); weapon.fireRate = 0; }
            if (!int.TryParse(parts[5], out weapon.bulletsPerShot)) { Debug.LogWarning($"WeaponLoader: Failed to parse bulletsPerShot for '{weapon.weaponName}' (value: '{parts[5]}'). Defaulting to 0."); weapon.bulletsPerShot = 0; }
            if (!float.TryParse(parts[6], out weapon.spreadAngle)) { Debug.LogWarning($"WeaponLoader: Failed to parse spreadAngle for '{weapon.weaponName}' (value: '{parts[6]}'). Defaulting to 0."); weapon.spreadAngle = 0; }

            weapon.spriteName = parts[7];

            // Find and assign the corresponding Sprite asset
            Sprite foundSprite = null;
            foreach (var sprite in weaponSprites)
            {
                // Ensure sprite in array is not null and its name matches the CSV spriteName
                if (sprite != null && sprite.name == weapon.spriteName)
                {
                    foundSprite = sprite;
                    break;
                }
            }
            weapon.weaponSprite = foundSprite;

            if (weapon.weaponSprite == null)
            {
                Debug.LogWarning($"WeaponLoader: Sprite not found for weapon: '{weapon.weaponName}' (Sprite name from CSV: '{weapon.spriteName}'). Ensure sprite is in 'Weapon Sprites' array and name matches exactly.");
            }

            loadedWeapons.Add(weapon); // Add the successfully parsed weapon to the list
        }

        // Convert the list of loaded weapons to an array
        weaponList.weapons = loadedWeapons.ToArray();

        // Send loaded data to the selection UI
        if (weaponSelectionUI != null)
        {
            Debug.Log($"[WeaponLoader - {gameObject.scene.name}] Initializing WeaponSelectionUI."); 
            weaponSelectionUI.InitializeWeapons(weaponList.weapons);
        }
        else
        {
            Debug.LogError("WeaponLoader: WeaponSelection UI reference is NULL in the Inspector! Cannot initialize UI.");
        }
    }
}
