using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 上班时间选择UI - 每天早上弹出选择时间段
/// 规则：
/// - 6个可勾选框（5个时段+正常上班）
/// - 8秒倒计时
/// - 正常上班=8-20四段，自选≥2段
/// </summary>
public class TimeSelectionUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Toggle[] timeToggles;      // 5个时段Toggle
    [SerializeField] private Toggle normalWorkToggle;    // 正常上班Toggle
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("配置")]
    [SerializeField] private float countdownTime = 8f;
    [SerializeField] private int minSelections = 2;

    private float currentCountdown;
    private bool isSelecting;
    private System.Action<List<TimeSegment>> onSelectionComplete;

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        // 设置Toggle事件
        if (normalWorkToggle != null)
        {
            normalWorkToggle.onValueChanged.AddListener(OnNormalWorkToggled);
        }

        for (int i = 0; i < timeToggles.Length; i++)
        {
            int index = i;
            if (timeToggles[i] != null)
            {
                timeToggles[i].onValueChanged.AddListener((isOn) => OnTimeToggled(index, isOn));
            }
        }

        // 初始隐藏
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isSelecting) return;

        // 用 unscaledDeltaTime，不受 timeScale 影响（游戏暂停时倒计时继续）
        currentCountdown -= Time.unscaledDeltaTime;

        if (countdownText != null)
        {
            countdownText.text = $"剩余时间: {Mathf.CeilToInt(currentCountdown)}秒";
        }

        if (currentCountdown <= 0)
        {
            OnTimeUp();
        }
    }

    /// <summary>
    /// 打开时间选择界面
    /// </summary>
    public void OpenSelection(System.Action<List<TimeSegment>> callback)
    {
        onSelectionComplete = callback;
        isSelecting = true;
        currentCountdown = countdownTime;

        // 重置所有Toggle
        ResetToggles();

        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }

        UpdateHint();
    }

    private void ResetToggles()
    {
        if (normalWorkToggle != null)
        {
            normalWorkToggle.isOn = true; // 默认选中正常上班
        }

        for (int i = 0; i < timeToggles.Length; i++)
        {
            if (timeToggles[i] != null)
            {
                timeToggles[i].isOn = false;
            }
        }
    }

    private void OnNormalWorkToggled(bool isOn)
    {
        if (isOn)
        {
            // 正常上班选中时，禁用其他选项
            for (int i = 0; i < timeToggles.Length; i++)
            {
                if (timeToggles[i] != null)
                {
                    timeToggles[i].interactable = false;
                    timeToggles[i].isOn = false;
                }
            }
        }
        else
        {
            // 正常上班取消时，启用其他选项
            for (int i = 0; i < timeToggles.Length; i++)
            {
                if (timeToggles[i] != null)
                {
                    timeToggles[i].interactable = true;
                }
            }
        }

        UpdateConfirmButton();
        UpdateHint();
    }

    private void OnTimeToggled(int index, bool isOn)
    {
        UpdateConfirmButton();
        UpdateHint();
    }

    private void UpdateConfirmButton()
    {
        if (confirmButton == null) return;

        if (normalWorkToggle != null && normalWorkToggle.isOn)
        {
            confirmButton.interactable = true;
            return;
        }

        // 检查是否选择了足够的时段
        int selectedCount = 0;
        for (int i = 0; i < timeToggles.Length; i++)
        {
            if (timeToggles[i] != null && timeToggles[i].isOn)
            {
                selectedCount++;
            }
        }

        confirmButton.interactable = selectedCount >= minSelections;
    }

    private void UpdateHint()
    {
        if (hintText == null) return;

        if (normalWorkToggle != null && normalWorkToggle.isOn)
        {
            hintText.text = "正常上班 (8:00-20:00)";
        }
        else
        {
            int selectedCount = 0;
            for (int i = 0; i < timeToggles.Length; i++)
            {
                if (timeToggles[i] != null && timeToggles[i].isOn)
                {
                    selectedCount++;
                }
            }

            hintText.text = $"已选择 {selectedCount}/{minSelections} 个时间段";
        }
    }

    private void OnConfirmClicked()
    {
        List<TimeSegment> selectedSegments = GetSelectedSegments();

        if (selectedSegments.Count >= minSelections ||
            (normalWorkToggle != null && normalWorkToggle.isOn))
        {
            isSelecting = false;

            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }

            onSelectionComplete?.Invoke(selectedSegments);
        }
    }

    private void OnTimeUp()
    {
        // 时间到，自动选择正常上班
        if (normalWorkToggle != null)
        {
            normalWorkToggle.isOn = true;
        }

        OnConfirmClicked();
    }

    private List<TimeSegment> GetSelectedSegments()
    {
        List<TimeSegment> segments = new List<TimeSegment>();

        if (normalWorkToggle != null && normalWorkToggle.isOn)
        {
            // 正常上班：8-20四个时段
            segments.Add(TimeSegment.Morning_8_11);
            segments.Add(TimeSegment.Noon_11_14);
            segments.Add(TimeSegment.Afternoon_14_17);
            segments.Add(TimeSegment.Dusk_17_20);
        }
        else
        {
            // 自定义选择
            TimeSegment[] allSegments = {
                TimeSegment.Morning_8_11,
                TimeSegment.Noon_11_14,
                TimeSegment.Afternoon_14_17,
                TimeSegment.Dusk_17_20,
                TimeSegment.Night_20_23
            };

            for (int i = 0; i < timeToggles.Length && i < allSegments.Length; i++)
            {
                if (timeToggles[i] != null && timeToggles[i].isOn)
                {
                    segments.Add(allSegments[i]);
                }
            }
        }

        return segments;
    }
}
