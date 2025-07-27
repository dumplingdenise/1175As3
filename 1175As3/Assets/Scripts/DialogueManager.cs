using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    // this dialogueManager is for keeping track of who is the current
    // npc dialogue set being runned since the npcs are using the same dialogue script
  
    
    public static DialogueManager instance;

    private NPC currentNPC; // for setting the current speaker

    void Awake()
    {
        // ensure only 1 dialogMnanager exists
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // for setting the current NPC
    public void RegisterNPC(NPC npc)
    {
        currentNPC = npc;
    }

    // for managing moving to next line of the active dialogue
    public void NextLine()
    {
        if (currentNPC != null)
        {
            currentNPC.NextLine();
        }
    }

    // for ending the npc's set of dialogues

    public void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
        }
    }

    // clear the reference to the current npc
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
