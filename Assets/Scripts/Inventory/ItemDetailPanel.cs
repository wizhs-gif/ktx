using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 物品详情面板 - 显示物品详细信息
/// </summary>
public class ItemDetailPanel : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemGroupText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TextMeshProUGUI lockedText;

    [Header("未知物品")]
    [SerializeField] private Sprite unknownSprite;
    [SerializeField] private string unknownName = "???";
    [SerializeField] private string unknownDescription = "尚未获得此物品";
    [SerializeField] private string unknownGroup = "未知";

    [Header("动画")]
    [SerializeField] private bool enableAnimation = true;
    [SerializeField] private float fadeInDuration = 0.2f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始隐藏
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示物品详情
    /// </summary>
    public void ShowDetail(ItemDatabase.ItemData itemData, bool isOwned)
    {
        gameObject.SetActive(true);

        if (enableAnimation)
        {
            StartCoroutine(FadeIn());
        }

        if (isOwned)
        {
            // 显示已拥有物品的详情
            ShowOwnedItem(itemData);
        }
        else
        {
            // 显示未知物品
            ShowLockedItem();
        }
    }

    private void ShowOwnedItem(ItemDatabase.ItemData itemData)
    {
        // 设置图标
        if (itemIcon != null)
        {
            itemIcon.sprite = itemData.icon;
            itemIcon.color = Color.white;
        }

        // 设置名称
        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }

        // 设置分组
        if (itemGroupText != null)
        {
            itemGroupText.text = itemData.group;
        }

        // 设置描述
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemData.description;
        }

        // 隐藏锁定遮罩
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(false);
        }
    }

    private void ShowLockedItem()
    {
        // 设置未知图标
        if (itemIcon != null && unknownSprite != null)
        {
            itemIcon.sprite = unknownSprite;
            itemIcon.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }

        // 设置未知名称
        if (itemNameText != null)
        {
            itemNameText.text = unknownName;
        }

        // 设置未知分组
        if (itemGroupText != null)
        {
            itemGroupText.text = unknownGroup;
        }

        // 设置未知描述
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = unknownDescription;
        }

        // 显示锁定遮罩
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(true);
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 关闭详情面板
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
