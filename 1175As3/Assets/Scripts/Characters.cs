using System;
using UnityEngine;

public class Characters : MonoBehaviour
{

    public TextAsset textAssetData;
    public Sprite[] characterSprites;

    [System.Serializable]
    public class Character
    {
        public string id;
        public string characterName;
        public string description;
        public float movementSpeed;
        public int maxHealth;
        /*public string initialWeaponId;*/
        public string characterSpriteName;
        public Sprite characterSprite;
    }

    [System.Serializable]
    public class characterList
    {
        public Character[] characters;
    }

    public characterList charactersList = new characterList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readCSV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void readCSV()
    {
        string[] data = textAssetData.text.Split(new string[] {"\n" }, StringSplitOptions.RemoveEmptyEntries); // split the file by rows and avoid empty rows



        if (data == null || data.Length <= 1)
        {
            Debug.LogError("File not found or no data after header");
            return;
        }

        charactersList.characters = new Character[data.Length - 1]; // subtract 1 for the header row

        for (int i = 1; i < data.Length; i++) // skip header
        {
            string line = data[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split(',');

            charactersList.characters[i - 1] = new Character();

            charactersList.characters[i - 1].id = parts[0];
            charactersList.characters[i - 1].characterName = parts[1];
            charactersList.characters[i - 1].description = parts[2];
            charactersList.characters[i- 1].movementSpeed = float.Parse(parts[3]);
            charactersList.characters[i - 1].maxHealth = int.Parse(parts[4]);
            /*charactersList.characters[i].initialWeaponId = parts[5];*/
            charactersList.characters[i - 1].characterSpriteName = parts[5];
            
            foreach (var charSprite in characterSprites)
            {
                string characterSprite = charSprite.name;
                if (characterSprite == parts[5])
                {
                    charactersList.characters[i - 1].characterSprite = charSprite;
                }
                else
                {
                    Debug.LogError("no sprite found");
                }
            }
        }
    }
}

