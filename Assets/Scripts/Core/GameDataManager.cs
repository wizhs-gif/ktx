using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局游戏数据管理器 - 存储所有游戏状态数据
/// 其他系统通过此类读写数据，通过 GameEvents 监听变化
/// </summary>
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // ==================== 精神值系统 ====================
    [Header("精神值")]
    [SerializeField] private int sanity = 5; // 初始满值（正常状态）
    public int Sanity => sanity;
    public const int SANITY_MIN = 0;
    public const int SANITY_MAX = 5;

    // ==================== 金钱系统 ====================
    [Header("金钱")]
    [SerializeField] private int money = 0;
    public int Money => money;

    // ==================== 时间系统 ====================
    [Header("时间")]
    [SerializeField] private int currentDay = 1;
    public int CurrentDay => currentDay;
    public const int DAY_MIN = 1;
    public const int DAY_MAX = 7;

    // ==================== 物品系统 ====================
    [Header("物品")]
    [SerializeField] private List<string> ownedItems = new List<string>();
    public IReadOnlyList<string> OwnedItems => ownedItems;

    // ==================== 游戏状态标记 ====================
    [Header("游戏标记")]
    [SerializeField] private Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();

    // ==================== 初始化 ====================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==================== 精神值操作 ====================

    /// <summary>
    /// 设置精神值（会自动钳制到0-5范围）
    /// </summary>
    public void SetSanity(int value)
    {
        int oldValue = sanity;
        sanity = Mathf.Clamp(value, SANITY_MIN, SANITY_MAX);

        if (oldValue != sanity)
        {
            Debug.Log($"[GameDataManager] 精神值变化: {oldValue} -> {sanity}");
            GameEvents.SanityChanged(oldValue, sanity);
        }
    }

    /// <summary>
    /// 增加精神值
    /// </summary>
    public void AddSanity(int amount)
    {
        SetSanity(sanity + amount);
    }

    /// <summary>
    /// 减少精神值
    /// </summary>
    public void ReduceSanity(int amount)
    {
        SetSanity(sanity - amount);
    }

    // ==================== 金钱操作 ====================

    /// <summary>
    /// 设置金钱
    /// </summary>
    public void SetMoney(int value)
    {
        int oldValue = money;
        money = Mathf.Max(0, value); // 金钱不能为负

        if (oldValue != money)
        {
            Debug.Log($"[GameDataManager] 金钱变化: {oldValue} -> {money}");
            GameEvents.MoneyChanged(oldValue, money);
        }
    }

    /// <summary>
    /// 增加金钱
    /// </summary>
    public void AddMoney(int amount)
    {
        SetMoney(money + amount);
    }

    /// <summary>
    /// 花费金钱（返回是否成功）
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            SetMoney(money - amount);
            return true;
        }
        Debug.LogWarning($"[GameDataManager] 金钱不足: 需要{amount}, 当前{money}");
        return false;
    }

    // ==================== 物品操作 ====================

    /// <summary>
    /// 检查是否拥有某物品
    /// </summary>
    public bool HasItem(string itemId)
    {
        return ownedItems.Contains(itemId);
    }

    /// <summary>
    /// 添加物品
    /// </summary>
    public void AddItem(string itemId)
    {
        if (!ownedItems.Contains(itemId))
        {
            ownedItems.Add(itemId);
            Debug.Log($"[GameDataManager] 获得物品: {itemId}");
            GameEvents.ItemObtained(itemId);
        }
    }

    /// <summary>
    /// 移除物品
    /// </summary>
    public bool RemoveItem(string itemId)
    {
        if (ownedItems.Remove(itemId))
        {
            Debug.Log($"[GameDataManager] 失去物品: {itemId}");
            GameEvents.ItemLost(itemId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取物品数量
    /// </summary>
    public int GetItemCount()
    {
        return ownedItems.Count;
    }

    // ==================== 时间操作 ====================

    /// <summary>
    /// 设置当前天数
    /// </summary>
    public void SetDay(int day)
    {
        int oldDay = currentDay;
        currentDay = Mathf.Clamp(day, DAY_MIN, DAY_MAX);

        if (oldDay != currentDay)
        {
            Debug.Log($"[GameDataManager] 天数变化: 第{oldDay}天 -> 第{currentDay}天");
            GameEvents.DayChanged(oldDay, currentDay);
        }
    }

    /// <summary>
    /// 推进到下一天
    /// </summary>
    public void AdvanceToNextDay()
    {
        if (currentDay < DAY_MAX)
        {
            SetDay(currentDay + 1);
        }
        else
        {
            Debug.Log("[GameDataManager] 已经是最后一天");
        }
    }

    /// <summary>
    /// 回到前一天
    /// </summary>
    public void GoToPreviousDay()
    {
        if (currentDay > DAY_MIN)
        {
            SetDay(currentDay - 1);
        }
        else
        {
            Debug.Log("[GameDataManager] 已经是第一天");
        }
    }

    // ==================== 游戏标记操作 ====================

    /// <summary>
    /// 设置游戏标记
    /// </summary>
    public void SetFlag(string flagName, bool value)
    {
        bool oldValue = GetFlag(flagName);
        gameFlags[flagName] = value;

        if (oldValue != value)
        {
            Debug.Log($"[GameDataManager] 标记变化: {flagName} = {value}");
            GameEvents.FlagChanged(flagName, value);
        }
    }

    /// <summary>
    /// 获取游戏标记
    /// </summary>
    public bool GetFlag(string flagName)
    {
        return gameFlags.TryGetValue(flagName, out bool value) && value;
    }

    /// <summary>
    /// 检查标记是否存在
    /// </summary>
    public bool HasFlag(string flagName)
    {
        return gameFlags.ContainsKey(flagName);
    }

    /// <summary>
    /// 获取所有游戏标记（用于存档）
    /// </summary>
    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(gameFlags);
    }

    // ==================== 物品批量操作 ====================

    /// <summary>
    /// 清空所有物品
    /// </summary>
    public void ClearAllItems()
    {
        ownedItems.Clear();
        Debug.Log("[GameDataManager] 所有物品已清空");
    }

    // ==================== 重置游戏数据 ====================

    /// <summary>
    /// 重置所有游戏数据（用于新游戏或重新开始）
    /// </summary>
    public void ResetAllData()
    {
        sanity = SANITY_MAX;
        money = 0;
        currentDay = 1;
        ownedItems.Clear();
        gameFlags.Clear();

        Debug.Log("[GameDataManager] 所有数据已重置");
    }

    /// <summary>
    /// 重置当天数据（保留物品和标记，重置时间）
    /// </summary>
    public void ResetDayData()
    {
        // 重置当天相关的标记
        List<string> flagsToRemove = new List<string>();
        foreach (var kvp in gameFlags)
        {
            if (kvp.Key.StartsWith("day_"))
            {
                flagsToRemove.Add(kvp.Key);
            }
        }
        foreach (var flag in flagsToRemove)
        {
            gameFlags.Remove(flag);
        }

        Debug.Log($"[GameDataManager] 第{currentDay}天数据已重置");
    }
}
