using UnityEngine;
using UnityEngine.Events;

public class NpcDialogueTrigger3D : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset csvFile;
    [SerializeField] private string defaultSpeaker = "NPC";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool playOnlyOnce = false;

    [Header("Prompt")]
    [SerializeField] private GameObject talkPromptRoot;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueStart;
    [SerializeField] private UnityEvent onDialogueComplete;

    private bool playerInRange = false;
    private bool alreadyPlayed = false;

    private void Awake()
    {
        SetPrompt(false);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (dialogueManager == null) return;
        if (dialogueManager.IsPlaying) return;
        if (playOnlyOnce && alreadyPlayed) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (csvFile == null) return;

        SetPrompt(false);
        onDialogueStart?.Invoke();

        dialogueManager.StartDialogue(csvFile, defaultSpeaker, HandleDialogueComplete);
    }

    private void HandleDialogueComplete()
    {
        alreadyPlayed = true;
        onDialogueComplete?.Invoke();

        if (playerInRange && !(playOnlyOnce && alreadyPlayed))
        {
            SetPrompt(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (dialogueManager != null && !dialogueManager.IsPlaying && !(playOnlyOnce && alreadyPlayed))
        {
            SetPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        SetPrompt(false);
    }

    private void SetPrompt(bool show)
    {
        if (talkPromptRoot != null)
            talkPromptRoot.SetActive(show);
    }
}