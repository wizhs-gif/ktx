using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC数据库 - 定义所有NPC的数据和出现条件
/// </summary>
[CreateAssetMenu(fileName = "NPCDatabase", menuName = "Game/NPC Database")]
public class NPCDatabase : ScriptableObject
{
    [System.Serializable]
    public class NPCData
    {
        public string id;               // NPC唯一ID
        public string npcName;          // NPC名称
        public string description;      // NPC描述
        public Sprite portrait;         // NPC立绘
        public GameObject prefab;       // NPC预制体

        [Header("出现条件")]
        public InteractCondition spawnCondition;

        [Header("对话数据")]
        public List<DialogueEntry> dialogues = new List<DialogueEntry>();
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string conditionFlag;    // 触发此对话的条件标记（空表示默认对话）
        public TextAsset csvFile;       // 对话CSV文件
        public InteractEffect effects;  // 对话完成后的效果
    }

    [Header("NPC列表")]
    [SerializeField] private List<NPCData> npcs = new List<NPCData>();

    private Dictionary<string, NPCData> npcLookup;

    /// <summary>
    /// 初始化查找表
    /// </summary>
    public void Initialize()
    {
        npcLookup = new Dictionary<string, NPCData>();
        foreach (var npc in npcs)
        {
            if (!string.IsNullOrEmpty(npc.id))
            {
                npcLookup[npc.id] = npc;
            }
        }
    }

    /// <summary>
    /// 根据ID获取NPC数据
    /// </summary>
    public NPCData GetNPC(string npcId)
    {
        if (npcLookup == null) Initialize();

        if (npcLookup.TryGetValue(npcId, out NPCData data))
        {
            return data;
        }

        Debug.LogWarning($"[NPCDatabase] 未找到NPC: {npcId}");
        return null;
    }

    /// <summary>
    /// 获取所有NPC
    /// </summary>
    public List<NPCData> GetAllNPCs()
    {
        return npcs;
    }

    /// <summary>
    /// 获取当前应该出现的NPC列表
    /// </summary>
    public List<NPCData> GetActiveNPCs()
    {
        List<NPCData> activeNPCs = new List<NPCData>();
        InteractSystem interactSystem = InteractSystem.Instance;

        foreach (var npc in npcs)
        {
            if (interactSystem.CheckConditions(npc.spawnCondition))
            {
                activeNPCs.Add(npc);
            }
        }

        return activeNPCs;
    }
}

/// <summary>
/// NPC ID常量定义
/// </summary>
public static class NPCIDs
{
    // 主线NPC
    public const string LU_HE = "lu_he";           // 陆禾（主角）
    public const string LIN_LIN = "lin_lin";        // 林霖
    public const string CHEN_SI_WEI = "chen_si_wei"; // 陈司微
    public const string LAO_TANG = "lao_tang";      // 老唐
    public const string JI_YI_NING = "ji_yi_ning";  // 纪以宁
    public const string TANG_YI = "tang_yi";        // 唐毅

    // 地图NPC
    public const string JI_YANG_FEI = "ji_yang_fei"; // 季阳飞
    public const string LIN_YUE = "lin_yue";         // 林悦
    public const string CHEN_WEI = "chen_wei";       // 陈微
    public const string NA_DI = "na_di";             // 娜汀

    // 通用NPC
    public const string GENERIC_MALE = "generic_male";
    public const string GENERIC_FEMALE = "generic_female";
}
