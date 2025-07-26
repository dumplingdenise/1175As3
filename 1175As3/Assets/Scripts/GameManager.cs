using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Characters.Character playerChar = SelectedCharacterManager.instance.selectedCharacter;

        if (playerChar == null)
        {
            Debug.LogError("No selected character found");
            return;
        }

        GameObject player = Instatiate(playerObject, )

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
