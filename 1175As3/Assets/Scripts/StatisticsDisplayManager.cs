using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//shumin
public class StatisticsDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI enemiesDefeatedText;
    public TextMeshProUGUI wavesCompletedText;
    public TextMeshProUGUI TotalDistanceTraveledText;
    public Button Back2MenuBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public void UpdateDisplay()
    {
        // check if the DynamicDataManager instance is available
        if (DynamicDataManager.Instance != null)
        {
            // Get and display enemies defeated, converting to string.
            enemiesDefeatedText.text = DynamicDataManager.Instance.GetEnemiesDefeated().ToString();

            // Get and display waves completed, converting to string.
            wavesCompletedText.text = DynamicDataManager.Instance.GetWavesCompleted().ToString();

            // Get and display total distance, formatted to two decimal places.
            TotalDistanceTraveledText.text = DynamicDataManager.Instance.GetTotalDistanceTraveled().ToString("F2");
        }
        else
        {
            Debug.LogError("DynamucDataManager not found! Statistics cannot be displayed.");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }
}
