using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//shumin
public class GameOverUIManager : MonoBehaviour
{
    public Button RestartButton;
    public Button BackToMenuButton;

    public void OnRestartButtonClick()
    {
        // Attempt to reset WaveManager if it exists and is accessible.
        // This provides a "belt-and-suspenders" approach to ensure a clean slate,
        // even if the WaveManager somehow persists unexpectedly across scene loads.
        if (WaveManager.Instance != null)
        {
            Debug.Log("[GameOverUIManager] Calling WaveManager.Instance.ResetWaveManager() before reloading GameScene.");
            WaveManager.Instance.ResetWaveManager();
        }
        else
        {
            Debug.LogWarning("[GameOverUIManager] WaveManager.Instance is null when attempting to reset it. This might be expected if the WaveManager is fully destroyed and recreated per scene load, or an issue if it's supposed to persist.");
        }

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
