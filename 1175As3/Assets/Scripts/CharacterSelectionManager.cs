using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public Image CharacterPortrait;
    public TextMeshProUGUI CharacterName;
    public TextMeshProUGUI CharacterHealth;
    public TextMeshProUGUI CharacterSpeed;
    public Button LeftButton;
    public Button RightButton;
    public Button PlayButton;
    public Button BackButton;

    public Characters CharactersData;  // Reference to the Characters script which loads character data

    private Characters.Character[] allCharacters; // hold all character data
    private int currentCharacterIndex = 0; // track which character is currently shown

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get all character data from the CharactersData script
        allCharacters = CharactersData.charactersList.characters;
        DisplayCurrentCharacter();

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

        CharacterName.text = selectedCharacter.characterName;

        CharacterHealth.text = "Health:" + selectedCharacter.maxHealth.ToString();

        CharacterSpeed.text = "Speed:" + selectedCharacter.movementSpeed.ToString();
    }

    public void OnLeftButtonClick()
    {
        currentCharacterIndex = 0;
    }
}
