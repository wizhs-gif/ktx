using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC控制器 - 管理NPC的行为和交互
/// </summary>
public class NPCController : MonoBehaviour
{
    [Header("NPC配置")]
    [SerializeField] private string npcId;
    [SerializeField] private string npcName;
    [SerializeField] private Sprite portrait;

    [Header("出现条件")]
    [SerializeField] private InteractCondition spawnCondition;

    [Header("对话数据")]
    [SerializeField] private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    [Header("交互")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [System.Serializable]
    public class DialogueEntry
    {
        public string conditionFlag;    // 条件标记
        public TextAsset csvFile;       // 对话CSV
        public InteractEffect effects;  // 对话效果
    }

    private bool playerInRange;
    private bool hasInteracted;

    private void Start()
    {
        // 检查出现条件
        if (InteractSystem.Instance != null)
        {
            bool shouldSpawn = InteractSystem.Instance.CheckConditions(spawnCondition);
            gameObject.SetActive(shouldSpawn);
        }

        // 隐藏交互提示
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void TryInteract()
    {
        // 查找符合条件的对话
        DialogueEntry validDialogue = GetValidDialogue();

        if (validDialogue != null && validDialogue.csvFile != null)
        {
            // 开始对话
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(
                    validDialogue.csvFile,
                    npcName,
                    () => OnDialogueComplete(validDialogue)
                );
            }
        }
    }

    private DialogueEntry GetValidDialogue()
    {
        if (GameDataManager.Instance == null) return null;

        // 查找第一个符合条件的对话
        foreach (var dialogue in dialogues)
        {
            // 如果没有条件，或者条件满足
            if (string.IsNullOrEmpty(dialogue.conditionFlag) ||
                GameDataManager.Instance.GetFlag(dialogue.conditionFlag))
            {
                return dialogue;
            }
        }

        return null;
    }

    private void OnDialogueComplete(DialogueEntry dialogue)
    {
        hasInteracted = true;

        // 应用对话效果
        if (dialogue.effects != null && InteractSystem.Instance != null)
        {
            InteractSystem.Instance.ExecuteEffects(dialogue.effects);
        }

        // 触发事件
        GameEvents.FlagChanged($"npc_{npcId}_talked", true);
    }

    /// <summary>
    /// 获取NPC ID
    /// </summary>
    public string GetNPCId()
    {
        return npcId;
    }

    /// <summary>
    /// 获取NPC名称
    /// </summary>
    public string GetNPCName()
    {
        return npcName;
    }
}
