using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite speakerPortrait;
        public string line;
    }

    public DialogueLine[] lines; // dialog lines
    public float typingSpeed = 0.05f; // speed of typewriter effect
    /*public AudioClip voiceSound;
    public float voicePitch = 0.5f;*/
    /*public float autoProgressDelay = 1.5f;*/ // Delay before auto-advancing
}
