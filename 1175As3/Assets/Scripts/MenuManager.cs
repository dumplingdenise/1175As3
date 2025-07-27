using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load to a new scene by the name
        SceneManager.LoadScene("CharacterSelection");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        //if in unity editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }

    public void SeeStats()
    {
        SceneManager.LoadScene("StatisticsScene");
    }
}
