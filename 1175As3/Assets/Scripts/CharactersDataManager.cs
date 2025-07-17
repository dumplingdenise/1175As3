/*using UnityEngine;
using System.Collections.Generic; // For List and Dictionary
using System.IO; // For file operations
using System.Linq; // For LINQ operations if needed (e.g., .ToDictionary())

public class CharactersDataManager : MonoBehaviour
{
    public static CharactersDataManager Instance { get; private set; }

    public List<Characters> AllCharacters { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    *//*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*//*

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        AllCharacters = new List<Characters>();

        TextAsset characterDataFile = Resources.Load<TextAsset>("characters");
        if (characterDataFile == null)
        {
            Debug.LogError("File not found");
        }

        string fileContent = characterDataFile.text;
        string[] lines = fileContent.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split(',');

            Characters character = new Characters
            {
                id = parts[0],
                characterName = parts[1],
                description = parts[2],
                movementSpeed = float.Parse(parts[3]),
                maxHealth = int.Parse(parts[4]),

            };

            AllCharacters.Add(character);
        }
    }
}
*/

/*TextAsset characterDataFile = Resources.Load<TextAsset>("characters");
        if (characterDataFile == null)
        {
            Debug.LogError("File not found");
        }*/


/*string fileContent = characterDataFile.text;
string[] lines = fileContent.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);*/