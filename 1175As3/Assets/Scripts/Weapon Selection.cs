using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WeaponSelectionManager : MonoBehaviour
{
    public Image weaponImage;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;
    public TextMeshProUGUI weaponDamageRate;
    public TextMeshProUGUI weaponFireRate;
    public TextMeshProUGUI bulletsPerShot;
    public TextMeshProUGUI spreadAngle;

    public Button leftButton;
    public Button rightButton;
    public Button selectButton;

    public WeaponLoader weaponLoader; // Reference to loader script
    private WeaponData[] allWeapons;
    private int currentWeaponIndex = 0;

    void Start()
    {
        allWeapons = weaponLoader.weaponList.weapons;

        if (allWeapons == null || allWeapons.Length == 0)
        {
            Debug.LogError("No weapons found!");
            return;
        }

        DisplayWeapon();
        UpdateArrowButtons();
    }

    void DisplayWeapon()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= allWeapons.Length)
        {
            Debug.LogError("Weapon index out of range!");
            return;
        }

        var w = allWeapons[currentWeaponIndex];
        weaponImage.sprite = w.weaponSprite;
        weaponName.text = w.weaponName;
        weaponDescription.text = w.description;
        weaponDamageRate.text = "Damage: " + w.damage;
        weaponFireRate.text = "Fire Rate: " + w.fireRate;
        bulletsPerShot.text = "Bullets/Shot: " + w.bulletsPerShot;
        spreadAngle.text = "Spread: " + w.spreadAngle;
    }

    public void OnLeftClick()
    {
        currentWeaponIndex = Mathf.Max(0, currentWeaponIndex - 1);
        DisplayWeapon();
        UpdateArrowButtons();
    }

    public void OnRightClick()
    {
        currentWeaponIndex = Mathf.Min(allWeapons.Length - 1, currentWeaponIndex + 1);
        DisplayWeapon();
        UpdateArrowButtons();
    }

    void UpdateArrowButtons()
    {
        leftButton.gameObject.SetActive(currentWeaponIndex > 0);
        rightButton.gameObject.SetActive(currentWeaponIndex < allWeapons.Length - 1);
    }

    public void OnPlayButtonClick()
    {
        // Save selected weapon index
        PlayerPrefs.SetInt("SelectedWeaponIndex", currentWeaponIndex);
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("StartGameMenu");
    }
}
