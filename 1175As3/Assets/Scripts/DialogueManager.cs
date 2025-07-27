using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    private NPC currentNPC;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterNPC(NPC npc)
    {
        currentNPC = npc;
    }

    public void NextLine()
    {
        if (currentNPC != null)
        {
            currentNPC.NextLine();
        }
    }

    public void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
        }
    }

    public void ClearNPC()
    {
        currentNPC = null;
    }

    public bool isDialogueOpen()
    {
        return currentNPC != null && currentNPC.IsDialogueActive();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
