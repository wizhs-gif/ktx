using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 物品组合UI - 家中"思考"功能的界面
/// </summary>
public class ItemCombineUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private ItemSlotUI slot1;
    [SerializeField] private ItemSlotUI slot2;
    [SerializeField] private ItemSlotUI resultSlot;
    [SerializeField] private Button combineButton;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("物品选择")]
    [SerializeField] private ItemSelectionPanel selectionPanel;

    private string selectedItem1;
    private string selectedItem2;

    private void Start()
    {
        if (combineButton != null)
        {
            combineButton.onClick.AddListener(OnCombineClicked);
            combineButton.interactable = false;
        }

        if (slot1 != null)
        {
            slot1.OnSlotClicked += () => OpenSelectionPanel(1);
        }

        if (slot2 != null)
        {
            slot2.OnSlotClicked += () => OpenSelectionPanel(2);
        }

        UpdateHint();
    }

    private void OpenSelectionPanel(int slotIndex)
    {
        if (selectionPanel != null)
        {
            selectionPanel.Open(slotIndex, OnItemSelected);
        }
    }

    private void OnItemSelected(int slotIndex, string itemId)
    {
        if (slotIndex == 1)
        {
            selectedItem1 = itemId;
            if (slot1 != null) slot1.SetItem(itemId);
        }
        else if (slotIndex == 2)
        {
            selectedItem2 = itemId;
            if (slot2 != null) slot2.SetItem(itemId);
        }

        UpdateCombineButton();
        UpdateResultPreview();
    }

    private void UpdateCombineButton()
    {
        if (combineButton != null)
        {
            bool canCombine = !string.IsNullOrEmpty(selectedItem1) && !string.IsNullOrEmpty(selectedItem2);
            combineButton.interactable = canCombine;
        }
    }

    private void UpdateResultPreview()
    {
        if (resultSlot == null) return;

        if (!string.IsNullOrEmpty(selectedItem1) && !string.IsNullOrEmpty(selectedItem2))
        {
            string result = ItemCombineSystem.Instance.GetCombineResult(selectedItem1, selectedItem2);
            if (!string.IsNullOrEmpty(result))
            {
                resultSlot.SetItem(result);
                resultSlot.SetPreview(true);
            }
            else
            {
                resultSlot.Clear();
                resultSlot.SetPreview(false);
            }
        }
        else
        {
            resultSlot.Clear();
            resultSlot.SetPreview(false);
        }
    }

    private void OnCombineClicked()
    {
        if (ItemCombineSystem.Instance == null) return;

        if (ItemCombineSystem.Instance.TryCombine(selectedItem1, selectedItem2, out string result))
        {
            // 组合成功
            if (resultSlot != null)
            {
                resultSlot.SetItem(result);
                resultSlot.PlaySuccessAnimation();
            }

            // 清空输入槽
            selectedItem1 = null;
            selectedItem2 = null;
            if (slot1 != null) slot1.Clear();
            if (slot2 != null) slot2.Clear();

            UpdateCombineButton();
            UpdateHint();

            Debug.Log($"[ItemCombineUI] 组合成功，获得: {result}");
        }
        else
        {
            // 组合失败
            if (hintText != null)
            {
                hintText.text = "这两个物品无法组合";
            }
        }
    }

    private void UpdateHint()
    {
        if (hintText != null)
        {
            hintText.text = "选择两个物品进行组合";
        }
    }

    /// <summary>
    /// 打开组合界面
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        selectedItem1 = null;
        selectedItem2 = null;

        if (slot1 != null) slot1.Clear();
        if (slot2 != null) slot2.Clear();
        if (resultSlot != null) resultSlot.Clear();

        UpdateCombineButton();
        UpdateHint();
    }

    /// <summary>
    /// 关闭组合界面
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}

/// <summary>
/// 物品槽UI组件
/// </summary>
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject emptyIndicator;
    [SerializeField] private GameObject previewOverlay;

    public event System.Action OnSlotClicked;

    private string itemId;

    public void SetItem(string id)
    {
        itemId = id;

        if (ItemDatabase.Instance != null)
        {
            var itemData = ItemDatabase.Instance.GetItem(id);
            if (itemData != null && itemData.icon != null)
            {
                itemIcon.sprite = itemData.icon;
                itemIcon.color = Color.white;
                if (emptyIndicator != null) emptyIndicator.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        itemId = null;
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        if (emptyIndicator != null) emptyIndicator.SetActive(true);
        SetPreview(false);
    }

    public void SetPreview(bool isPreview)
    {
        if (previewOverlay != null)
        {
            previewOverlay.SetActive(isPreview);
        }
    }

    public void PlaySuccessAnimation()
    {
        // 播放成功动画
    }

    public void OnClick()
    {
        OnSlotClicked?.Invoke();
    }
}
