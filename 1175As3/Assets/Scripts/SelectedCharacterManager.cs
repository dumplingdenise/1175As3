// denise
using UnityEngine;

public class SelectedCharacterManager : MonoBehaviour
{
    public static SelectedCharacterManager instance;

    public Characters.Character selectedCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // keep across scenes
        }
        else
        {
            Destroy(this.gameObject); // if duplicate, remove
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
