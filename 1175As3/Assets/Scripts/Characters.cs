using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Characters : MonoBehaviour
{

    /*public TextAsset textAssetData;*/
    public Sprite[] allCharacterSprites;

    [System.Serializable]
    public class Character
    {
        public string id;
        public string characterName;
        public string description;
        public float movementSpeed;
        public int maxHealth;
        public int armorRating;
        public string characterSpriteName;
        public Sprite defaultCharacterSprite; // for the normal sprite
        public Dictionary<string, List<Sprite>> movementSprite = new(); // for sprites like walking, jumping, etc
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
        string filePath = Application.dataPath + "/characters.csv";

        if (!File.Exists(filePath))
        {
            Debug.LogError("CHARACTER CSV FILE NOT FOUND");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);


        // to count number of elements inside the files excluding header & empty lines
        int count = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            if (!string.IsNullOrEmpty(lines[i]))
            {
                count++;
            }
        }

        charactersList.characters = new Character[count];

        int index = 0; // starts from index 0 of the array
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) // check if is empty line
            {
                continue; // if empty skip rest of code and go to next iteration
            }

            string[] parts = lines[i].Split(",");

            charactersList.characters[index] = new Character()
            {
                id = parts[0],
                characterName = parts[1],
                description = parts[2],
                movementSpeed = float.Parse(parts[3]),
                maxHealth = int.Parse(parts[4]),
                armorRating = int.Parse(parts[5]),
                characterSpriteName = parts[6],
                movementSprite = new Dictionary<string, List<Sprite>>()
            };

            // get all the sprites related to that character based on the sprite name in excel
            foreach (var sprite in allCharacterSprites)
            {
                if (!sprite.name.StartsWith(charactersList.characters[index].characterSpriteName))
                {
                    continue;
                }

                string suffix = sprite.name.Substring(charactersList.characters[index].characterSpriteName.Length).TrimStart('_'); // extract 

                if (suffix == "0") // if the suffix is exactly 0 = to defaultCharacterSprite
                {
                    charactersList.characters[index].defaultCharacterSprite = sprite;
                    continue;
                }

                // extract key from suffix
                string key = suffix.Split('_')[0];

                if (!charactersList.characters[index].movementSprite.ContainsKey(key))
                {
                    charactersList.characters[index].movementSprite[key] = new List<Sprite>(); // create new list if key does not exist
                }

                charactersList.characters[index].movementSprite[key].Add(sprite); // add sprite to the list
            }

            foreach (var key in charactersList.characters[index].movementSprite.Keys.ToList())
            {
                charactersList.characters[index].movementSprite[key] = charactersList.characters[index].movementSprite[key].OrderBy(x => x.name).ToList(); // sort the list by asceding order
            }
            index++;
        }
    }
}

