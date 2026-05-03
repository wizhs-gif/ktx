using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 物品选择面板 - 用于选择物品进行组合
/// </summary>
public class ItemSelectionPanel : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Transform itemGridParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("配置")]
    [SerializeField] private ItemDatabase itemDatabase;

    private List<ItemSelectionSlot> slots = new List<ItemSelectionSlot>();
    private Action<int, string> onItemSelected;
    private int currentSlotIndex;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开选择面板
    /// </summary>
    public void Open(int slotIndex, Action<int, string> callback)
    {
        currentSlotIndex = slotIndex;
        onItemSelected = callback;

        gameObject.SetActive(true);

        if (titleText != null)
        {
            titleText.text = $"选择物品放入槽位{slotIndex}";
        }

        RefreshDisplay();
    }

    /// <summary>
    /// 关闭选择面板
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void RefreshDisplay()
    {
        if (itemDatabase == null || GameDataManager.Instance == null) return;

        // 清空现有槽位
        ClearSlots();

        // 获取拥有的物品
        List<string> ownedItems = new List<string>(GameDataManager.Instance.OwnedItems);

        // 创建物品槽
        foreach (string itemId in ownedItems)
        {
            var itemData = itemDatabase.GetItem(itemId);
            if (itemData == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, itemGridParent);
            ItemSelectionSlot slot = slotObj.GetComponent<ItemSelectionSlot>();

            if (slot != null)
            {
                slot.Initialize(itemData);
                slot.OnClicked += OnItemClicked;
                slots.Add(slot);
            }
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.OnClicked -= OnItemClicked;
                Destroy(slot.gameObject);
            }
        }
        slots.Clear();
    }

    private void OnItemClicked(string itemId)
    {
        onItemSelected?.Invoke(currentSlotIndex, itemId);
        Close();
    }
}

/// <summary>
/// 物品选择槽
/// </summary>
public class ItemSelectionSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;

    public event Action<string> OnClicked;

    private string itemId;

    public void Initialize(ItemDatabase.ItemData itemData)
    {
        itemId = itemData.id;

        if (itemIcon != null && itemData.icon != null)
        {
            itemIcon.sprite = itemData.icon;
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }
    }

    public void OnClick()
    {
        OnClicked?.Invoke(itemId);
    }
}
