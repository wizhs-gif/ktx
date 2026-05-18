using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 地铁选站UI - 纯UI界面，点击站点切换场景
/// 支持天数限制、UI变暗/噪点等效果
/// </summary>
public class SubwayMapUI : MonoBehaviour
{
    [Serializable]
    public class StationConfig
    {
        public string stationName;          // 站点显示名
        public string sceneName;            // 对应场景名
        public Button stationButton;        // 站点按钮
        public int availableFromDay = 1;    // 从第几天开始可点击
    }

    [Header("站点配置")]
    [SerializeField] private List<StationConfig> stations = new List<StationConfig>();

    [Header("UI元素")]
    [SerializeField] private GameObject subwayPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI stationTooltip;

    [Header("视觉效果")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float dimAmount = 0.4f;        // 第4天变暗程度
    [SerializeField] private Material noiseMaterial;         // 噪点材质

    [Header("开场剧情")]
    [SerializeField] private string[] openingDayTexts;       // 开场每天的字幕
    [SerializeField] private int[] openingHighlightStations; // 开场高亮的站点索引
    [SerializeField] private bool openingInteractable = true;// 开场是否可交互

    private int currentDay;
    private Action<string> onStationSelected;

    private void Start()
    {
        // 绑定按钮事件
        for (int i = 0; i < stations.Count; i++)
        {
            int index = i;
            if (stations[i].stationButton != null)
            {
                stations[i].stationButton.onClick.AddListener(() => OnStationClicked(index));

                // 鼠标悬停显示站点名
                var trigger = stations[i].stationButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                enterEntry.callback.AddListener((_) => ShowTooltip(stations[index].stationName));
                trigger.triggers.Add(enterEntry);

                var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
                exitEntry.callback.AddListener((_) => HideTooltip());
                trigger.triggers.Add(exitEntry);
            }
        }

        HideTooltip();
    }

    /// <summary>
    /// 打开地铁界面
    /// </summary>
    public void OpenSubway(Action<string> callback)
    {
        onStationSelected = callback;

        if (GameDataManager.Instance != null)
        {
            currentDay = GameDataManager.Instance.CurrentDay;
        }

        if (subwayPanel != null)
        {
            subwayPanel.SetActive(true);
        }

        UpdateStationAvailability();
        ApplyVisualEffects();

        if (hintText != null)
        {
            hintText.text = "选择目的地";
        }
    }

    /// <summary>
    /// 用于开场剧情：设置特定状态
    /// </summary>
    public void SetOpeningState(int dayIndex, string dayText, int highlightIndex, bool interactable)
    {
        if (subwayPanel != null)
        {
            subwayPanel.SetActive(true);
        }

        // 先禁用所有站点
        for (int i = 0; i < stations.Count; i++)
        {
            if (stations[i].stationButton != null)
            {
                stations[i].stationButton.interactable = false;
                var colors = stations[i].stationButton.colors;
                colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                stations[i].stationButton.colors = colors;
            }
        }

        // 高亮指定站点
        if (highlightIndex >= 0 && highlightIndex < stations.Count && stations[highlightIndex].stationButton != null)
        {
            var btn = stations[highlightIndex].stationButton;
            btn.interactable = interactable;
            var colors = btn.colors;
            colors.normalColor = Color.white;
            btn.colors = colors;
        }

        if (hintText != null)
        {
            hintText.text = dayText;
        }

        // 开场视觉效果（根据天数递进）
        ApplyOpeningVisuals(dayIndex);
    }

    private void UpdateStationAvailability()
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (stations[i].stationButton != null)
            {
                bool available = currentDay >= stations[i].availableFromDay;
                stations[i].stationButton.interactable = available;

                var colors = stations[i].stationButton.colors;
                colors.normalColor = available ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.5f);
                stations[i].stationButton.colors = colors;
            }
        }
    }

    private void ApplyVisualEffects()
    {
        if (backgroundImage == null) return;

        if (currentDay >= 5)
        {
            // 第5天起：不可交互，直接跳转
            // 由外部逻辑处理
        }
        else if (currentDay >= 4)
        {
            // 第4天：UI加噪点
            if (noiseMaterial != null)
            {
                backgroundImage.material = noiseMaterial;
            }
            backgroundImage.color = new Color(dimAmount, dimAmount, dimAmount, 1f);
        }
        else if (currentDay >= 3)
        {
            // 第3天：UI变暗
            backgroundImage.color = new Color(dimAmount, dimAmount, dimAmount, 1f);
        }
        else
        {
            // 正常
            backgroundImage.color = Color.white;
            backgroundImage.material = null;
        }
    }

    private void ApplyOpeningVisuals(int dayIndex)
    {
        if (backgroundImage == null) return;

        switch (dayIndex)
        {
            case 0: // 第1天：正常
            case 1: // 第2天：正常
                backgroundImage.color = Color.white;
                backgroundImage.material = null;
                break;
            case 2: // 第3天：变暗
                backgroundImage.color = new Color(dimAmount, dimAmount, dimAmount, 1f);
                backgroundImage.material = null;
                break;
            case 3: // 第4天：噪点
                backgroundImage.color = new Color(dimAmount, dimAmount, dimAmount, 1f);
                if (noiseMaterial != null) backgroundImage.material = noiseMaterial;
                break;
            case 4: // 第5天：更暗+噪点
                backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                if (noiseMaterial != null) backgroundImage.material = noiseMaterial;
                break;
        }
    }

    private void OnStationClicked(int index)
    {
        if (index < 0 || index >= stations.Count) return;

        string sceneName = stations[index].sceneName;
        Debug.Log($"[SubwayMapUI] 选择站点: {stations[index].stationName} → {sceneName}");

        CloseSubway();
        onStationSelected?.Invoke(sceneName);
    }

    private void ShowTooltip(string stationName)
    {
        if (stationTooltip != null)
        {
            stationTooltip.text = stationName;
            stationTooltip.gameObject.SetActive(true);
        }
    }

    private void HideTooltip()
    {
        if (stationTooltip != null)
        {
            stationTooltip.gameObject.SetActive(false);
        }
    }

    private void CloseSubway()
    {
        if (subwayPanel != null)
        {
            subwayPanel.SetActive(false);
        }
    }
}
