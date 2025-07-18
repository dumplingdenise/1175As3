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

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
       HealthBarSlider.maxValue = maxHealth; // set maximum value of the health bar

       HealthBarSlider.value = currentHealth; // set current value of the health bar
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
        Time.timeScale = 1.0f; //ensure game time is normal
        SceneManager.LoadScene("StartGameMenu");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
