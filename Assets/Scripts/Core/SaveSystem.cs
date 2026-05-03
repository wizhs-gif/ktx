using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存档系统 - 负责游戏数据的持久化存储
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private const string SAVE_KEY = "GameSaveData";

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

    // ==================== 存档数据结构 ====================

    [System.Serializable]
    private class SaveData
    {
        public int sanity;
        public int money;
        public int currentDay;
        public List<string> ownedItems = new List<string>();
        public List<FlagEntry> flags = new List<FlagEntry>();
    }

    [System.Serializable]
    private class FlagEntry
    {
        public string key;
        public bool value;

        public FlagEntry(string key, bool value)
        {
            this.key = key;
            this.value = value;
        }
    }

    // ==================== 保存游戏 ====================

    /// <summary>
    /// 保存当前游戏状态
    /// </summary>
    public void SaveGame()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("[SaveSystem] GameDataManager不存在，无法保存");
            return;
        }

        SaveData data = new SaveData();
        GameDataManager gdm = GameDataManager.Instance;

        // 保存基本数据
        data.sanity = gdm.Sanity;
        data.money = gdm.Money;
        data.currentDay = gdm.CurrentDay;

        // 保存物品列表
        data.ownedItems = new List<string>(gdm.OwnedItems);

        // 保存游戏标记
        // 注意：Dictionary无法直接序列化，需要转换
        // 这里需要在GameDataManager中添加获取所有标记的方法

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[SaveSystem] 游戏已保存");
    }

    // ==================== 读取存档 ====================

    /// <summary>
    /// 读取存档并应用到GameDataManager
    /// </summary>
    public bool LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[SaveSystem] 没有找到存档");
            return false;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogError("[SaveSystem] GameDataManager不存在，无法读取");
            return false;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogError("[SaveSystem] 存档数据解析失败");
            return false;
        }

        GameDataManager gdm = GameDataManager.Instance;

        // 恢复基本数据
        gdm.SetSanity(data.sanity);
        gdm.SetMoney(data.money);
        gdm.SetDay(data.currentDay);

        // 恢复物品（先清空再添加）
        // 注意：这里需要在GameDataManager中添加清空物品的方法
        foreach (string itemId in data.ownedItems)
        {
            gdm.AddItem(itemId);
        }

        // 恢复游戏标记
        foreach (FlagEntry flag in data.flags)
        {
            gdm.SetFlag(flag.key, flag.value);
        }

        Debug.Log("[SaveSystem] 存档已加载");
        return true;
    }

    // ==================== 删除存档 ====================

    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] 存档已删除");
    }

    /// <summary>
    /// 检查是否有存档
    /// </summary>
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    // ==================== 自动保存 ====================

    /// <summary>
    /// 在每日结束时自动保存
    /// </summary>
    public void AutoSaveOnDayEnd()
    {
        SaveGame();
        Debug.Log("[SaveSystem] 每日自动保存完成");
    }
}
