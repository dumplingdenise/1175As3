using UnityEngine;

public class DynamicDataManager : MonoBehaviour
{
    public static DynamicDataManager Instance;

    private int enemiesdefeated;
    private int wavescompleted;
    private int itemsused;

    void Awake()
    {
        //check if an instance already exists
        if(Instance == null)
        {
            //set this instance as the singleton
            Instance = this;

            //make this gameobject persist across scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //if there is another instance that exists, destroy this instance
            Destroy(gameObject);
        }

        enemiesdefeated = 0;
        wavescompleted = 0;
        itemsused = 0;
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementEnemiesDefeated()
    {
        enemiesdefeated++;
    }

    public void IncrementWavesCompleted()
    {
        wavescompleted++;
    }

    public void IncrementItemUsed()
    {
        itemsused++;
    }

    public int GetEnemiesDefeated()
    {
        return enemiesdefeated;
    }

    public int GetWavesCompleted()
    {
        return wavescompleted;
    }

    public int GetItemUsed()
    {
        return itemsused;
    }
}
