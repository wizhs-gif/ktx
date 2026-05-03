using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 时间显示组件 - 显示日期和时间段
/// </summary>
public class TimeDisplay : MonoBehaviour
{
    [Header("日期显示")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private string dateFormat = "{0}月{1}日 {2}";

    [Header("时间段显示")]
    [SerializeField] private TextMeshProUGUI timeSegmentText;
    [SerializeField] private Image timeSegmentIcon;

    [Header("时间段图标")]
    [SerializeField] private Sprite morningIcon;
    [SerializeField] private Sprite noonIcon;
    [SerializeField] private Sprite afternoonIcon;
    [SerializeField] private Sprite duskIcon;
    [SerializeField] private Sprite nightIcon;

    [Header("时间段颜色")]
    [SerializeField] private Color morningColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private Color noonColor = new Color(1f, 1f, 0.9f);
    [SerializeField] private Color afternoonColor = new Color(1f, 0.9f, 0.7f);
    [SerializeField] private Color duskColor = new Color(1f, 0.7f, 0.5f);
    [SerializeField] private Color nightColor = new Color(0.4f, 0.4f, 0.6f);

    [Header("星期显示")]
    [SerializeField] private bool showWeekday = true;
    [SerializeField] private string[] weekdayNames = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

    private void OnEnable()
    {
        GameEvents.OnDayChanged += OnDayChanged;
        GameEvents.OnTimeSegmentChanged += OnTimeSegmentChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnDayChanged -= OnDayChanged;
        GameEvents.OnTimeSegmentChanged -= OnTimeSegmentChanged;
    }

    private void Start()
    {
        // 初始化显示
        if (GameDataManager.Instance != null)
        {
            UpdateDateDisplay(GameDataManager.Instance.CurrentDay);
        }

        if (TimeManager.Instance != null)
        {
            UpdateTimeSegmentDisplay(TimeManager.Instance.CurrentSegment);
        }
    }

    private void OnDayChanged(int oldDay, int newDay)
    {
        UpdateDateDisplay(newDay);
    }

    private void OnTimeSegmentChanged(TimeSegment segment)
    {
        UpdateTimeSegmentDisplay(segment);
    }

    private void UpdateDateDisplay(int day)
    {
        if (dateText != null)
        {
            // 根据策划案，游戏从3月17日开始（周二）
            int startDay = 17 + day - 1;
            int month = 3;
            if (startDay > 31)
            {
                startDay -= 31;
                month = 4;
            }

            string weekday = "";
            if (showWeekday)
            {
                // 3月17日是周二，计算星期几
                int weekdayIndex = (day - 1 + 1) % 7; // +1因为17号是周二（索引1）
                weekday = weekdayNames[weekdayIndex];
            }

            dateText.text = string.Format(dateFormat, month, startDay, weekday);
        }
    }

    private void UpdateTimeSegmentDisplay(TimeSegment segment)
    {
        if (timeSegmentText != null)
        {
            switch (segment)
            {
                case TimeSegment.Morning_8_11:
                    timeSegmentText.text = "早晨";
                    timeSegmentText.color = morningColor;
                    if (timeSegmentIcon != null && morningIcon != null) timeSegmentIcon.sprite = morningIcon;
                    break;
                case TimeSegment.Noon_11_14:
                    timeSegmentText.text = "中午";
                    timeSegmentText.color = noonColor;
                    if (timeSegmentIcon != null && noonIcon != null) timeSegmentIcon.sprite = noonIcon;
                    break;
                case TimeSegment.Afternoon_14_17:
                    timeSegmentText.text = "下午";
                    timeSegmentText.color = afternoonColor;
                    if (timeSegmentIcon != null && afternoonIcon != null) timeSegmentIcon.sprite = afternoonIcon;
                    break;
                case TimeSegment.Dusk_17_20:
                    timeSegmentText.text = "傍晚";
                    timeSegmentText.color = duskColor;
                    if (timeSegmentIcon != null && duskIcon != null) timeSegmentIcon.sprite = duskIcon;
                    break;
                case TimeSegment.Night_20_23:
                    timeSegmentText.text = "夜晚";
                    timeSegmentText.color = nightColor;
                    if (timeSegmentIcon != null && nightIcon != null) timeSegmentIcon.sprite = nightIcon;
                    break;
            }
        }
    }

    /// <summary>
    /// 获取时间段描述文本
    /// </summary>
    public static string GetTimeSegmentName(TimeSegment segment)
    {
        switch (segment)
        {
            case TimeSegment.Morning_8_11: return "8:00-11:00";
            case TimeSegment.Noon_11_14: return "11:00-14:00";
            case TimeSegment.Afternoon_14_17: return "14:00-17:00";
            case TimeSegment.Dusk_17_20: return "17:00-20:00";
            case TimeSegment.Night_20_23: return "20:00-23:00";
            default: return "";
        }
    }
}
