using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据库 - 定义所有物品的数据
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public static ItemDatabase Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    [System.Serializable]
    public class ItemData
    {
        public string id;           // 物品唯一ID
        public string itemName;     // 物品名称
        public string description;  // 物品描述
        public string group;        // 所属组（回忆组、植物组等）
        public Sprite icon;         // 物品图标
        public bool isKey;          // 是否为关键物品
    }

    [Header("物品列表")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    // 物品ID到数据的映射
    private Dictionary<string, ItemData> itemLookup;

    /// <summary>
    /// 初始化查找表
    /// </summary>
    public void Initialize()
    {
        itemLookup = new Dictionary<string, ItemData>();
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.id))
            {
                itemLookup[item.id] = item;
            }
        }
    }

    /// <summary>
    /// 根据ID获取物品数据
    /// </summary>
    public ItemData GetItem(string itemId)
    {
        if (itemLookup == null) Initialize();

        if (itemLookup.TryGetValue(itemId, out ItemData data))
        {
            return data;
        }

        Debug.LogWarning($"[ItemDatabase] 未找到物品: {itemId}");
        return null;
    }

    /// <summary>
    /// 获取所有物品
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return items;
    }

    /// <summary>
    /// 获取指定组的所有物品
    /// </summary>
    public List<ItemData> GetItemsByGroup(string group)
    {
        return items.FindAll(x => x.group == group);
    }

    /// <summary>
    /// 检查物品是否存在
    /// </summary>
    public bool HasItem(string itemId)
    {
        if (itemLookup == null) Initialize();
        return itemLookup.ContainsKey(itemId);
    }
}

/// <summary>
/// 物品组常量定义
/// </summary>
public static class ItemGroups
{
    public const string MEMORY = "回忆组";
    public const string PLANT = "植物组";
    public const string MYSTERY = "志异组";
    public const string RECORD = "记录组";
    public const string HEART = "心组";
    public const string TIME = "时间组";
    public const string MEDICINE = "药物组";
    public const string COFFEE = "咖啡组";
}

/// <summary>
/// 物品ID常量定义
/// </summary>
public static class ItemIDs
{
    // 回忆组
    public const string MEMORY_COLORLESS = "memory_colorless";
    public const string MEMORY_BLUE = "memory_blue";
    public const string MEMORY_CYAN = "memory_cyan";
    public const string MEMORY_BLOOD = "memory_blood";
    public const string MEMORY_GOLD = "memory_gold";
    public const string MEMORY_BLACK = "memory_black";

    // 植物组
    public const string PLANT_MAGNOLIA = "plant_magnolia";
    public const string PLANT_PINECONE = "plant_pinecone";
    public const string PLANT_BULB = "plant_bulb";
    public const string PLANT_REDSCALE = "plant_redscale";
    public const string PLANT_GOLDPEN = "plant_goldpen";

    // 志异组
    public const string MYSTERY_ZHENHAO = "mystery_zhenhao";
    public const string MYSTERY_INCENSE = "mystery_incense";
    public const string MYSTERY_FORTUNE = "mystery_fortune";
    public const string MYSTERY_YAO = "mystery_yao";
    public const string MYSTERY_HAIR = "mystery_hair";

    // 记录组
    public const string RECORD_NEWSPAPER = "record_newspaper";
    public const string RECORD_POSTER = "record_poster";
    public const string RECORD_TREATMENT = "record_treatment";

    // 心组
    public const string HEART_LEAVE = "heart_leave";
    public const string HEART_SKETCH = "heart_sketch";
    public const string HEART_HAT = "heart_hat";
    public const string HEART_PROMISE = "heart_promise";
    public const string HEART_RICE = "heart_rice";

    // 时间组
    public const string TIME_MIRROR = "time_mirror";
    public const string TIME_BROKEN_MIRROR = "time_broken_mirror";
    public const string TIME_CAMERA = "time_camera";
    public const string TIME_FUR = "time_fur";
    public const string TIME_STRANGE_PACKAGE = "time_strange_package";
    public const string TIME_HOME_PACKAGE = "time_home_package";

    // 药物组
    public const string MEDICINE_CHECK = "medicine_check";
    public const string MEDICINE_ROBOT = "medicine_robot";

    // 咖啡组
    public const string COFFEE_1 = "coffee_1";
    public const string COFFEE_2 = "coffee_2";
    public const string COFFEE_3 = "coffee_3";
    public const string COFFEE_4 = "coffee_4";
    public const string COFFEE_5 = "coffee_5";
}
