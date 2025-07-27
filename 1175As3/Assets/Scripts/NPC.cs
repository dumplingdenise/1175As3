using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public Dialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    public GameObject closeBtn;
    public GameObject nextBtn;

    public GameObject promptPanel;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public tutorialCharMovement playerScript;

    public bool shouldFadeAndDisappear = false; 

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
        {
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }

        else
        {
            DialogueManager.instance.RegisterNPC(this);
            StartDialogue();
        }

    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        UpdateSpeakerUI();

        dialoguePanel.SetActive(true);
        promptPanel.SetActive(false);
        
        UpdateButtonVisibility();
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.lines[dialogueIndex].line);
            isTyping = false;
        }
        ++dialogueIndex;
        if (dialogueIndex < dialogueData.lines.Length)
        {
            // if another line, type next line
            UpdateSpeakerUI();
            StartCoroutine(TypeLine());
            UpdateButtonVisibility();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.lines[dialogueIndex].line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        if (playerScript != null)
        {
            playerScript.canMove = true; // set to enable movement after dialogue
        }

        if (shouldFadeAndDisappear == true)
        {
            StartCoroutine(FadeOutAndDisappear(0.5f));
        }

        DialogueManager.instance.ClearNPC(); 
    }

    void UpdateSpeakerUI()
    {
        var currentLine = dialogueData.lines[dialogueIndex];
        nameText.SetText(currentLine.speakerName);
        portraitImage.sprite = currentLine.speakerPortrait;
    }

    private IEnumerator FadeOutAndDisappear(float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        gameObject.SetActive(false); // hide npc
    }

    void UpdateButtonVisibility()
    {
        bool isLastLine = dialogueIndex == dialogueData.lines.Length - 1;

        if (closeBtn != null)
        {
            closeBtn.SetActive(isLastLine);
        }
        if (nextBtn != null)
        {
            nextBtn.SetActive(!isLastLine);
        }
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
