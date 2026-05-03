using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交互系统 - 管理所有交互点的条件判断和执行
/// </summary>
public class InteractSystem : MonoBehaviour
{
    public static InteractSystem Instance { get; private set; }

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

    // ==================== 交互条件检查 ====================

    /// <summary>
    /// 检查交互条件是否满足
    /// </summary>
    public bool CheckConditions(InteractCondition conditions)
    {
        if (conditions == null) return true;

        GameDataManager gdm = GameDataManager.Instance;
        if (gdm == null) return false;

        // 检查天数条件
        if (conditions.requiredDay > 0 && gdm.CurrentDay != conditions.requiredDay)
            return false;

        if (conditions.minDay > 0 && gdm.CurrentDay < conditions.minDay)
            return false;

        if (conditions.maxDay > 0 && gdm.CurrentDay > conditions.maxDay)
            return false;

        // 检查精神值条件
        if (conditions.minSanity >= 0 && gdm.Sanity < conditions.minSanity)
            return false;

        if (conditions.maxSanity >= 0 && gdm.Sanity > conditions.maxSanity)
            return false;

        // 检查物品条件
        foreach (string requiredItem in conditions.requiredItems)
        {
            if (!gdm.HasItem(requiredItem))
                return false;
        }

        foreach (string forbiddenItem in conditions.forbiddenItems)
        {
            if (gdm.HasItem(forbiddenItem))
                return false;
        }

        // 检查游戏标记条件
        foreach (var flag in conditions.requiredFlags)
        {
            if (!gdm.GetFlag(flag))
                return false;
        }

        return true;
    }

    // ==================== 交互执行 ====================

    /// <summary>
    /// 执行交互效果
    /// </summary>
    public void ExecuteEffects(InteractEffect effects)
    {
        if (effects == null) return;

        GameDataManager gdm = GameDataManager.Instance;
        if (gdm == null) return;

        // 精神值变化
        if (effects.sanityChange != 0)
        {
            gdm.AddSanity(effects.sanityChange);
        }

        // 金钱变化
        if (effects.moneyChange != 0)
        {
            gdm.AddMoney(effects.moneyChange);
        }

        // 添加物品
        foreach (string itemId in effects.addItems)
        {
            gdm.AddItem(itemId);
        }

        // 移除物品
        foreach (string itemId in effects.removeItems)
        {
            gdm.RemoveItem(itemId);
        }

        // 设置游戏标记
        foreach (var flag in effects.setFlags)
        {
            gdm.SetFlag(flag, true);
        }

        Debug.Log("[InteractSystem] 交互效果已执行");
    }
}

/// <summary>
/// 交互条件数据类
/// </summary>
[System.Serializable]
public class InteractCondition
{
    [Header("天数条件")]
    public int requiredDay = -1;    // 要求特定天数（-1表示不限）
    public int minDay = -1;         // 最小天数
    public int maxDay = -1;         // 最大天数

    [Header("精神值条件")]
    public int minSanity = -1;      // 最小精神值（-1表示不限）
    public int maxSanity = -1;      // 最大精神值

    [Header("物品条件")]
    public List<string> requiredItems = new List<string>();  // 必须拥有的物品
    public List<string> forbiddenItems = new List<string>(); // 不能拥有的物品

    [Header("游戏标记条件")]
    public List<string> requiredFlags = new List<string>();  // 必须为true的标记
}

/// <summary>
/// 交互效果数据类
/// </summary>
[System.Serializable]
public class InteractEffect
{
    [Header("属性变化")]
    public int sanityChange = 0;
    public int moneyChange = 0;

    [Header("物品变化")]
    public List<string> addItems = new List<string>();
    public List<string> removeItems = new List<string>();

    [Header("游戏标记")]
    public List<string> setFlags = new List<string>();
}
