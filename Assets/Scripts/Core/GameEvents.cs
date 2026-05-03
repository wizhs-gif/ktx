using System;

/// <summary>
/// 全局事件中心 - 所有系统通过此类通信，避免直接引用
/// </summary>
public static class GameEvents
{
    // ==================== 精神值相关 ====================
    /// <summary>精神值变化 (旧值, 新值)</summary>
    public static event Action<int, int> OnSanityChanged;
    public static void SanityChanged(int oldVal, int newVal) => OnSanityChanged?.Invoke(oldVal, newVal);

    // ==================== 金钱相关 ====================
    /// <summary>金钱变化 (旧值, 新值)</summary>
    public static event Action<int, int> OnMoneyChanged;
    public static void MoneyChanged(int oldVal, int newVal) => OnMoneyChanged?.Invoke(oldVal, newVal);

    // ==================== 物品相关 ====================
    /// <summary>获得物品 (物品ID)</summary>
    public static event Action<string> OnItemObtained;
    public static void ItemObtained(string itemId) => OnItemObtained?.Invoke(itemId);

    /// <summary>失去物品 (物品ID)</summary>
    public static event Action<string> OnItemLost;
    public static void ItemLost(string itemId) => OnItemLost?.Invoke(itemId);

    // ==================== 时间相关 ====================
    /// <summary>天数变化 (旧天数, 新天数)</summary>
    public static event Action<int, int> OnDayChanged;
    public static void DayChanged(int oldDay, int newDay) => OnDayChanged?.Invoke(oldDay, newDay);

    /// <summary>时间段变化 (新时间段)</summary>
    public static event Action<TimeSegment> OnTimeSegmentChanged;
    public static void TimeSegmentChanged(TimeSegment segment) => OnTimeSegmentChanged?.Invoke(segment);

    // ==================== 游戏流程 ====================
    /// <summary>每日开始</summary>
    public static event Action<int> OnDayStarted;
    public static void DayStarted(int day) => OnDayStarted?.Invoke(day);

    /// <summary>每日结束</summary>
    public static event Action<int> OnDayEnded;
    public static void DayEnded(int day) => OnDayEnded?.Invoke(day);

    /// <summary>游戏状态标记变化 (标记名, 值)</summary>
    public static event Action<string, bool> OnFlagChanged;
    public static void FlagChanged(string flagName, bool value) => OnFlagChanged?.Invoke(flagName, value);

    // ==================== 清除所有事件（用于重新开始） ====================
    public static void ClearAll()
    {
        OnSanityChanged = null;
        OnMoneyChanged = null;
        OnItemObtained = null;
        OnItemLost = null;
        OnDayChanged = null;
        OnTimeSegmentChanged = null;
        OnDayStarted = null;
        OnDayEnded = null;
        OnFlagChanged = null;
    }
}
