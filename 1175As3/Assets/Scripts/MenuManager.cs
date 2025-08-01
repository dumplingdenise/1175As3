using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// shumin
public class MenuManager : MonoBehaviour
{
    public GameObject loadGameBtn;

    //Bonus
     void Start()
    {
        if (loadGameBtn != null)
        {
            if (DynamicDataManager.Instance != null)
            {
                // Set the button's active state based on whether a save file exists
                loadGameBtn.SetActive(DynamicDataManager.Instance.DoesSaveFileExist());
            }
            else
            {
                loadGameBtn.SetActive(false);
            }

        }
    }
    public void StartGame()
    {
        // Load to a new scene by the name
        SceneManager.LoadScene("CharacterSelection");
    }

    //Bonus 
    public void StartNewGame()
    {
        //call the method in dynamicdatamanager to reset all stats
        if(DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.StartNewGame(); // reset the data and delete file
        }

        SceneManager.LoadScene("Tutorial");
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

    // Call this function to load the "Tutorial" scene
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial"); // "Tutorial" should be the exact name of your scene
    }


}
