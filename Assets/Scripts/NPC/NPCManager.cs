using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC管理器 - 管理场景中所有NPC的生成和行为
/// </summary>
public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    [Header("NPC配置")]
    [SerializeField] private NPCDatabase npcDatabase;
    [SerializeField] private Transform npcParent;

    [Header("NPC预制体")]
    [SerializeField] private GameObject npcPrefab;

    private List<NPCController> activeNPCs = new List<NPCController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // 监听天数和精神值变化
        GameEvents.OnDayChanged += OnDayChanged;
        GameEvents.OnSanityChanged += OnSanityChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged -= OnDayChanged;
        GameEvents.OnSanityChanged -= OnSanityChanged;
    }

    private void Start()
    {
        // 初始化NPC
        SpawnNPCs();
    }

    private void OnDayChanged(int oldDay, int newDay)
    {
        // 天数变化时重新生成NPC
        RefreshNPCs();
    }

    private void OnSanityChanged(int oldVal, int newVal)
    {
        // 精神值变化时检查NPC状态
        RefreshNPCs();
    }

    /// <summary>
    /// 生成NPC
    /// </summary>
    public void SpawnNPCs()
    {
        if (npcDatabase == null || InteractSystem.Instance == null) return;

        // 清空现有NPC
        ClearNPCs();

        // 获取应该出现的NPC
        List<NPCDatabase.NPCData> activeNPCData = npcDatabase.GetActiveNPCs();

        foreach (var npcData in activeNPCData)
        {
            SpawnNPC(npcData);
        }
    }

    private void SpawnNPC(NPCDatabase.NPCData npcData)
    {
        if (npcData.prefab == null && npcPrefab == null) return;

        // 确定生成位置
        Vector3 spawnPosition = GetNPCSpawnPosition(npcData.id);

        // 生成NPC
        GameObject npcObj = npcData.prefab != null ?
            Instantiate(npcData.prefab, spawnPosition, Quaternion.identity, npcParent) :
            Instantiate(npcPrefab, spawnPosition, Quaternion.identity, npcParent);

        // 设置NPC数据
        NPCController controller = npcObj.GetComponent<NPCController>();
        if (controller != null)
        {
            activeNPCs.Add(controller);
        }

        Debug.Log($"[NPCManager] 生成NPC: {npcData.npcName}");
    }

    private Vector3 GetNPCSpawnPosition(string npcId)
    {
        // 根据NPC ID返回对应的生成位置
        // 这里需要根据实际场景配置
        switch (npcId)
        {
            case NPCIDs.JI_YANG_FEI:
                return new Vector3(10f, 0f, 5f); // 医院住院楼前
            case NPCIDs.LIN_YUE:
                return new Vector3(-8f, 0f, 3f); // 门诊楼前
            case NPCIDs.CHEN_WEI:
                return new Vector3(5f, 0f, -2f); // 门诊楼前
            case NPCIDs.NA_DI:
                return new Vector3(0f, 0f, 8f);  // 咖啡馆
            default:
                return Vector3.zero;
        }
    }

    /// <summary>
    /// 刷新NPC状态
    /// </summary>
    public void RefreshNPCs()
    {
        // 检查现有NPC是否还应该存在
        for (int i = activeNPCs.Count - 1; i >= 0; i--)
        {
            if (activeNPCs[i] == null)
            {
                activeNPCs.RemoveAt(i);
                continue;
            }

            // 这里可以添加更复杂的NPC状态检查逻辑
        }

        // 重新生成应该出现的NPC
        SpawnNPCs();
    }

    /// <summary>
    /// 清空所有NPC
    /// </summary>
    public void ClearNPCs()
    {
        foreach (var npc in activeNPCs)
        {
            if (npc != null)
            {
                Destroy(npc.gameObject);
            }
        }
        activeNPCs.Clear();
    }

    /// <summary>
    /// 获取指定NPC
    /// </summary>
    public NPCController GetNPC(string npcId)
    {
        return activeNPCs.Find(n => n.GetNPCId() == npcId);
    }

    /// <summary>
    /// 检查NPC是否存在
    /// </summary>
    public bool IsNPCActive(string npcId)
    {
        return activeNPCs.Exists(n => n.GetNPCId() == npcId);
    }
}
