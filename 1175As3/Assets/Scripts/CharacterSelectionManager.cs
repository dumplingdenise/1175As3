// denise
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography.X509Certificates;

//shumin, denise
public class CharacterSelectionManager : MonoBehaviour
{
    public Image CharacterPortrait;
    public TextMeshProUGUI CharacterName;
    public TextMeshProUGUI CharacterDescription;
    public TextMeshProUGUI CharacterHealth;
    public TextMeshProUGUI CharacterSpeed;
    public TextMeshProUGUI CharacterArmorRating;
    public Button LeftButton;
    public Button RightButton;
    public Button PlayButton;
    public Button BackButton;

    public Characters CharactersData;  // Reference to the Characters script which loads character data

    private Characters.Character[] allCharacters; // hold all character data
    private int currentCharacterIndex = 0; // track which character is currently shown
    internal string characterName;
    internal float movementSpeed;
    internal int maxHealth;
    internal int armorRating;
    internal object defaultCharacterSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get all character data from the CharactersData script
        allCharacters = CharactersData.charactersList.characters;
        DisplayCurrentCharacter(); //display first character on scene
        UpdateArrowButtons();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayCurrentCharacter()
    {
        // Check if character data was loaded successfully and is not empty
        if (allCharacters == null || allCharacters.Length == 0)
        {
            Debug.LogError("No character data loaded!");
            return;
        } 

        //ensure index is within valid bounds
        if (currentCharacterIndex < 0 || currentCharacterIndex >= allCharacters.Length)
        {
            Debug.LogError("Character index out of bounds:" + currentCharacterIndex);
            return;
        }

        Characters.Character selectedCharacter = allCharacters[currentCharacterIndex];

        //Update Text
        CharacterPortrait.sprite = selectedCharacter.defaultCharacterSprite;

        CharacterName.text = selectedCharacter.characterName;

        CharacterDescription.text = selectedCharacter.description;

        CharacterHealth.text = "Health:" + selectedCharacter.maxHealth.ToString();

        CharacterSpeed.text = "Speed:" + selectedCharacter.movementSpeed.ToString();

        CharacterArmorRating.text = "Armor Rating:" + selectedCharacter.armorRating.ToString();
    }

    public void OnLeftButtonClick()
    {
        currentCharacterIndex = currentCharacterIndex -1;

        currentCharacterIndex = Mathf.Max(0, currentCharacterIndex); // ensure Index does not go below 0

        DisplayCurrentCharacter(); // update the UI

        UpdateArrowButtons();
    }

    public void OnRightButtonClick()
    {
        currentCharacterIndex = currentCharacterIndex + 1;

        currentCharacterIndex = Mathf.Min(allCharacters.Length - 1, currentCharacterIndex); //prevent index from exceeding max

        DisplayCurrentCharacter();

        UpdateArrowButtons();
    }

    private void UpdateArrowButtons()
    {
        // Check if character data was loaded successfully and is not empty
        if (allCharacters == null || allCharacters.Length == 0)
        {
            LeftButton.gameObject.SetActive(false);
            RightButton.gameObject.SetActive(false);
            return;
        }

        if (currentCharacterIndex == 0)
        {
            /*LeftButton.interactable = false;*/
            LeftButton.gameObject.SetActive(false); //disable left button if at first character
        }
        else
        {
            /*LeftButton.interactable = true;*/
            LeftButton.gameObject.SetActive(true);
        }

        if (currentCharacterIndex == allCharacters.Length - 1)
        {
            /*RightButton.interactable = false;*/
            RightButton.gameObject.SetActive(false); // disable right button if at last character
        }
        else
        {
           /* RightButton.interactable = true;*/
           RightButton.gameObject.SetActive(true);
        }
    }

    public void OnPlayButtonClick()
    {
        SelectedCharacterManager.instance.selectedCharacter = allCharacters[currentCharacterIndex];
        SceneManager.LoadScene("WeaponSelection"); // to be change once the game scene is created
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("Main");
    }
}
