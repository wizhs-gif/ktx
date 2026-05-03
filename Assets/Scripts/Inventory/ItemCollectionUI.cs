using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 物品图鉴UI - 显示物品收集状态
/// </summary>
public class ItemCollectionUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform itemGridParent;      // 物品网格父物体
    [SerializeField] private GameObject itemSlotPrefab;      // 物品槽预制体
    [SerializeField] private ItemDetailPanel detailPanel;    // 详情面板

    [Header("分组标签")]
    [SerializeField] private ToggleGroup groupToggleGroup;
    [SerializeField] private Toggle allToggle;
    [SerializeField] private Toggle memoryToggle;
    [SerializeField] private Toggle plantToggle;
    [SerializeField] private Toggle mysteryToggle;
    [SerializeField] private Toggle recordToggle;
    [SerializeField] private Toggle heartToggle;
    [SerializeField] private Toggle timeToggle;
    [SerializeField] private Toggle medicineToggle;
    [SerializeField] private Toggle coffeeToggle;

    [Header("统计显示")]
    [SerializeField] private TextMeshProUGUI collectionCountText;
    [SerializeField] private string countFormat = "{0}/{1}";

    [Header("配置")]
    [SerializeField] private ItemDatabase itemDatabase;

    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private string currentFilter = "";

    private void OnEnable()
    {
        GameEvents.OnItemObtained += OnItemObtained;
        RefreshDisplay();
    }

    private void OnDisable()
    {
        GameEvents.OnItemObtained -= OnItemObtained;
    }

    private void Start()
    {
        // 初始化分组标签
        InitializeGroupToggles();

        // 刷新显示
        RefreshDisplay();
    }

    private void InitializeGroupToggles()
    {
        if (allToggle != null)
            allToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(""); });
        if (memoryToggle != null)
            memoryToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.MEMORY); });
        if (plantToggle != null)
            plantToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.PLANT); });
        if (mysteryToggle != null)
            mysteryToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.MYSTERY); });
        if (recordToggle != null)
            recordToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.RECORD); });
        if (heartToggle != null)
            heartToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.HEART); });
        if (timeToggle != null)
            timeToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.TIME); });
        if (medicineToggle != null)
            medicineToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.MEDICINE); });
        if (coffeeToggle != null)
            coffeeToggle.onValueChanged.AddListener((isOn) => { if (isOn) FilterByGroup(ItemGroups.COFFEE); });
    }

    private void OnItemObtained(string itemId)
    {
        RefreshDisplay();
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    public void RefreshDisplay()
    {
        if (itemDatabase == null || GameDataManager.Instance == null) return;

        // 清空现有槽位
        ClearSlots();

        // 获取所有物品
        List<ItemDatabase.ItemData> allItems = itemDatabase.GetAllItems();
        List<string> ownedItems = new List<string>(GameDataManager.Instance.OwnedItems);

        // 创建物品槽
        foreach (var item in allItems)
        {
            // 应用筛选
            if (!string.IsNullOrEmpty(currentFilter) && item.group != currentFilter)
                continue;

            // 创建槽位
            GameObject slotObj = Instantiate(itemSlotPrefab, itemGridParent);
            ItemSlot slot = slotObj.GetComponent<ItemSlot>();

            if (slot != null)
            {
                bool isOwned = ownedItems.Contains(item.id);
                slot.Initialize(item, isOwned);
                slot.OnItemSelected += ShowItemDetail;
                itemSlots.Add(slot);
            }
        }

        // 更新统计
        UpdateCollectionCount();
    }

    private void ClearSlots()
    {
        foreach (var slot in itemSlots)
        {
            if (slot != null)
            {
                slot.OnItemSelected -= ShowItemDetail;
                Destroy(slot.gameObject);
            }
        }
        itemSlots.Clear();
    }

    private void FilterByGroup(string group)
    {
        currentFilter = group;
        RefreshDisplay();
    }

    private void ShowItemDetail(ItemDatabase.ItemData itemData, bool isOwned)
    {
        if (detailPanel != null)
        {
            detailPanel.ShowDetail(itemData, isOwned);
        }
    }

    private void UpdateCollectionCount()
    {
        if (collectionCountText != null && itemDatabase != null)
        {
            int totalItems = itemDatabase.GetAllItems().Count;
            int ownedItems = GameDataManager.Instance.OwnedItems.Count;
            collectionCountText.text = string.Format(countFormat, ownedItems, totalItems);
        }
    }

    /// <summary>
    /// 打开图鉴
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        RefreshDisplay();
    }

    /// <summary>
    /// 关闭图鉴
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
