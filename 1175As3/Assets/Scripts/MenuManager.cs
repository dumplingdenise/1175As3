using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load to a new scene by the name
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSlection");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        //if in unity editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
