using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 每日结束UI - 显示一天结束后的选项
/// 选项：前往明天 / 重开今天 / 回到昨天
/// </summary>
public class DayEndUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject dayEndPanel;
    [SerializeField] private TextMeshProUGUI dayEndText;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button replayDayButton;
    [SerializeField] private Button previousDayButton;

    [Header("配置")]
    [SerializeField] private string dayEndFormat = "第{0}天结束";
    [SerializeField] private string nextDayText = "前往明天";
    [SerializeField] private string replayDayText = "重开今天";
    [SerializeField] private string previousDayText = "回到昨天";

    private System.Action<DayEndChoice> onChoiceMade;

    private void Start()
    {
        // 设置按钮事件
        if (nextDayButton != null)
        {
            nextDayButton.onClick.AddListener(() => OnChoiceMade(DayEndChoice.NextDay));
        }

        if (replayDayButton != null)
        {
            replayDayButton.onClick.AddListener(() => OnChoiceMade(DayEndChoice.ReplayDay));
        }

        if (previousDayButton != null)
        {
            previousDayButton.onClick.AddListener(() => OnChoiceMade(DayEndChoice.PreviousDay));
        }

        // 初始隐藏
        if (dayEndPanel != null)
        {
            dayEndPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示每日结束界面
    /// </summary>
    public void ShowDayEnd(System.Action<DayEndChoice> callback)
    {
        onChoiceMade = callback;

        // 更新文本
        if (dayEndText != null && GameDataManager.Instance != null)
        {
            dayEndText.text = string.Format(dayEndFormat, GameDataManager.Instance.CurrentDay);
        }

        // 更新按钮文本
        if (nextDayButton != null)
        {
            var buttonText = nextDayButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = nextDayText;
        }

        if (replayDayButton != null)
        {
            var buttonText = replayDayButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = replayDayText;
        }

        if (previousDayButton != null)
        {
            var buttonText = previousDayButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = previousDayText;

            // 第一天不能回到昨天
            previousDayButton.interactable = GameDataManager.Instance != null &&
                                             GameDataManager.Instance.CurrentDay > 1;
        }

        if (dayEndPanel != null)
        {
            dayEndPanel.SetActive(true);
        }
    }

    private void OnChoiceMade(DayEndChoice choice)
    {
        if (dayEndPanel != null)
        {
            dayEndPanel.SetActive(false);
        }

        onChoiceMade?.Invoke(choice);
    }
}

/// <summary>
/// 每日结束选择
/// </summary>
public enum DayEndChoice
{
    NextDay,      // 前往明天
    ReplayDay,    // 重开今天
    PreviousDay   // 回到昨天
}
