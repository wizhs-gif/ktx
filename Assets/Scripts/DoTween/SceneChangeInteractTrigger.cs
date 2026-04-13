using UnityEngine;

public class SceneChangeInteractTrigger : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactPrompt;

    [Header("Transition")]
    [SerializeField] private SceneTransitionController transitionController;
    [SerializeField] private string nextSceneName;
    [TextArea]
    [SerializeField] private string subtitleLine;

    private bool playerInRange = false;

    private void Awake()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (transitionController == null) return;
        if (transitionController.IsPlaying) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);

            transitionController.PlayTransitionAndLoad(nextSceneName, subtitleLine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}