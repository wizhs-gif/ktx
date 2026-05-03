using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 物品槽组件 - 显示单个物品
/// </summary>
public class ItemSlot : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject newIcon;

    [Header("颜色配置")]
    [SerializeField] private Color ownedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0.8f);
    [SerializeField] private Color normalBorderColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color selectedBorderColor = Color.yellow;

    [Header("未知物品")]
    [SerializeField] private Sprite unknownSprite;

    private ItemDatabase.ItemData itemData;
    private bool isOwned;
    private bool isSelected;

    public event Action<ItemDatabase.ItemData, bool> OnItemSelected;

    /// <summary>
    /// 初始化物品槽
    /// </summary>
    public void Initialize(ItemDatabase.ItemData data, bool owned)
    {
        itemData = data;
        isOwned = owned;

        // 设置图标
        if (itemIcon != null)
        {
            if (isOwned && data.icon != null)
            {
                itemIcon.sprite = data.icon;
                itemIcon.color = ownedColor;
            }
            else if (unknownSprite != null)
            {
                itemIcon.sprite = unknownSprite;
                itemIcon.color = lockedColor;
            }
        }

        // 设置背景
        if (backgroundImage != null)
        {
            backgroundImage.color = isOwned ? ownedColor : lockedColor;
        }

        // 显示/隐藏锁定图标
        if (lockIcon != null)
        {
            lockIcon.SetActive(!isOwned);
        }

        // 隐藏新物品图标（需要额外逻辑判断是否为新获得）
        if (newIcon != null)
        {
            newIcon.SetActive(false);
        }

        // 设置边框
        UpdateBorder(false);
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnClick()
    {
        OnItemSelected?.Invoke(itemData, isOwned);
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBorder(selected);
    }

    private void UpdateBorder(bool selected)
    {
        if (borderImage != null)
        {
            borderImage.color = selected ? selectedBorderColor : normalBorderColor;
        }
    }

    /// <summary>
    /// 获取物品数据
    /// </summary>
    public ItemDatabase.ItemData GetItemData()
    {
        return itemData;
    }

    /// <summary>
    /// 是否已拥有
    /// </summary>
    public bool IsOwned()
    {
        return isOwned;
    }

    /// <summary>
    /// 标记为新物品
    /// </summary>
    public void MarkAsNew(bool isNew)
    {
        if (newIcon != null)
        {
            newIcon.SetActive(isNew);
        }
    }
}
