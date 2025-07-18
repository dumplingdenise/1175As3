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
