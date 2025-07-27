using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameOverUIManager : MonoBehaviour
{
    public Button RestartButton;
    public Button BackToMenuButton;

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackToMenuButtonClick()
    {
        //ensure dynamic datamanager instance exist before trying to save
        if(DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.SaveGameStats();
            Debug.Log("Game stats saved when returning to main menu from Game Over screen.");
        }
        else
        {
            Debug.LogError("DynamicDataManager instance not found! Cannot save game stats.");
        }

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
