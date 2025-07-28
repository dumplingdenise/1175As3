// shumin
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameUIManager : MonoBehaviour
{
    public Slider HealthBarSlider;
    public TextMeshProUGUI ScoreText;
    public Button PauseButton;
    public GameObject PauseMenuPanel;
    public Button ResumeButton;
    public Button MainMenuButton;

    public TextMeshProUGUI HealthScore;

    public Slider ArmorRatingSlider;
    public TextMeshProUGUI ArmorRating;

    /*private Characters.Character playerCharacter;*/
    private int currentHealth;
    private int currentArmor;

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
       HealthBarSlider.maxValue = maxHealth; // set maximum value of the health bar

       HealthBarSlider.value = currentHealth; // set current value of the health bar

       HealthScore.text = currentHealth.ToString();
    }

    public void UpdateArmor(int currentArmor, int maxArmor)
    {
        ArmorRatingSlider.maxValue = maxArmor; // set maximum value of the 

        ArmorRatingSlider.value = currentArmor; // set current value of the 

        ArmorRating.text = currentArmor.ToString();
    }

    public void UpdateScore(int score)
    {
        ScoreText.text = "Score:" + score.ToString();
    }

    public void OnPauseButtonClick()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.0f; //Pause game
            PauseMenuPanel.SetActive(true);
            PauseButton.gameObject.SetActive(false);
        }
        else
        {
            OnResumeButtonClick();
        }
    }

    public void OnResumeButtonClick()
    {
        Time.timeScale = 1.0f; //resume game
        PauseMenuPanel.SetActive(false);
        PauseButton.gameObject.SetActive(true);
    }

    public void OnBack2MainMenuButtonClick()
    {
        // Ensure DynamicDataManager instance exists before trying to save
        if (DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.SaveGameStats(); // This line saves the game data
            Debug.Log("Game stats saved when returning to main menu from in-game.");
        }
        else
        {
            Debug.LogError("DynamicDataManager instance not found! Cannot save game stats.");
        }

        Time.timeScale = 1.0f; //ensure game time is normal
        SceneManager.LoadScene("Main");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = SelectedCharacterManager.instance.selectedCharacter.maxHealth;
        HealthScore.text = currentHealth.ToString();

        currentArmor = SelectedCharacterManager.instance.selectedCharacter.armorRating;
        ArmorRating.text = currentArmor.ToString();

        UpdateHealth(currentHealth, SelectedCharacterManager.instance.selectedCharacter.maxHealth);
        UpdateArmor(currentArmor, SelectedCharacterManager.instance.selectedCharacter.armorRating);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
