using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WeaponSelection : MonoBehaviour
{
    public Image weaponImage;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;
    public TextMeshProUGUI weaponDamage; // Corrected to match your current script
    public TextMeshProUGUI weaponFireRate;
    public TextMeshProUGUI bulletsPerShot;
    public TextMeshProUGUI spreadAngle;

    public Button leftButton;   // Renamed
    public Button rightButton;  // Renamed
    public Button selectButton; // Renamed from PlayButton
    public Button backButton;   // Renamed

    public WeaponLoader weaponLoader;

    // No need for a direct WeaponLoader reference here if WeaponLoader calls InitializeWeapons

    private WeaponData[] allWeapons;
    private int currentWeaponIndex = 0;

    // This method is called EXTERNALLY by WeaponLoader once data is ready
    public void InitializeWeapons(WeaponData[] weapons)
    {
        allWeapons = weapons;

        if (allWeapons == null || allWeapons.Length == 0)
        {
            Debug.LogError("WeaponSelection: No weapons found after initialization!");
            // Consider disabling UI elements if no weapons are found
            weaponImage.gameObject.SetActive(false);
            weaponName.gameObject.SetActive(false);
            weaponDescription.gameObject.SetActive(false);
            weaponDamage.gameObject.SetActive(false);
            weaponFireRate.gameObject.SetActive(false);
            bulletsPerShot.gameObject.SetActive(false);
            spreadAngle.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            return;
        }

        currentWeaponIndex = 0;
        DisplayCurrentWeapon(); // Renamed for clarity and consistency
        UpdateArrowButtons();
    }

    // Renamed for clarity and consistency
    private void DisplayCurrentWeapon()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= allWeapons.Length)
        {
            Debug.LogError("WeaponSelection: Weapon index out of bounds: " + currentWeaponIndex);
            return;
        }

        WeaponData weapon = allWeapons[currentWeaponIndex];
        weaponImage.sprite = weapon.weaponSprite;
        weaponName.text = weapon.weaponName;
        weaponDescription.text = weapon.description;
        // Ensure 'w.damage' is correctly assigned to 'weaponDamageRate'
        weaponDamage.text = "Damage: " + weapon.damage.ToString();
        weaponFireRate.text = "Fire Rate: " + weapon.fireRate.ToString();
        bulletsPerShot.text = "Bullets/Shot: " + weapon.bulletsPerShot.ToString();
        spreadAngle.text = "Spread: " + weapon.spreadAngle.ToString();
    }

    // Public methods for button clicks
    public void OnLeftClick() // Renamed from OnLeftButtonClick
    {
        currentWeaponIndex = Mathf.Max(0, currentWeaponIndex - 1);
        DisplayCurrentWeapon();
        UpdateArrowButtons();
    }

    public void OnRightClick() // Renamed from OnRightButtonClick
    {
        currentWeaponIndex = Mathf.Min(allWeapons.Length - 1, currentWeaponIndex + 1);
        DisplayCurrentWeapon();
        UpdateArrowButtons();
    }

    // Renamed for clarity and consistency
    private void UpdateArrowButtons()
    {
        // Handle case where allWeapons might be null or empty if initialization failed
        if (allWeapons == null || allWeapons.Length == 0)
        {
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            return;
        }

        leftButton.gameObject.SetActive(currentWeaponIndex > 0);
        rightButton.gameObject.SetActive(currentWeaponIndex < allWeapons.Length - 1);
    }

    public void OnSelectButtonClick() // Renamed from OnPlayButtonClick
    {
        // This will call the ResetWaveManager method on the existing, persistent WaveManager instance.
        if (WaveManager.Instance != null)
        {
            Debug.Log("[GameOverUIManager] Calling WaveManager.Instance.ResetWaveManager() before reloading GameScene.");
            WaveManager.Instance.ResetWaveManager();
        }
        else
        {
            Debug.LogWarning("[GameOverUIManager] WaveManager.Instance is null when trying to reset it from GameOver screen. This should not happen if WaveManager is on a DontDestroyOnLoad GameObject.");
        }

        PlayerPrefs.SetInt("SelectedWeaponIndex", currentWeaponIndex);
        SelectedWeapon.instance.selectedWeapon = allWeapons[currentWeaponIndex];
        SceneManager.LoadScene("GameScene"); // Retained original game scene load
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("CharacterSelection"); // Your updated back scene
    }
}