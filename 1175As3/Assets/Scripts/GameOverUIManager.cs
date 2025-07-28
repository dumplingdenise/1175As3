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

        SceneManager.LoadScene("Main");
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
