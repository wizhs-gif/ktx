using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD管理器 - 显示游戏核心信息（精神值、金钱、时间、地图名称）
/// 挂载在HUD Canvas上
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("精神值显示")]
    [SerializeField] private Image[] sanityIcons;          // 精神值图标数组（6个）
    [SerializeField] private Color sanityActiveColor = Color.white;
    [SerializeField] private Color sanityInactiveColor = Color.gray;
    [SerializeField] private Sprite sanityNormalSprite;    // 正常精神值图标
    [SerializeField] private Sprite sanityWarningSprite;   // 警告精神值图标
    [SerializeField] private Sprite sanityDangerSprite;    // 危险精神值图标

    [Header("金钱显示")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string moneyFormat = "¥{0}";

    [Header("时间显示")]
    [SerializeField] private TextMeshProUGUI dateText;      // 日期文本
    [SerializeField] private TextMeshProUGUI timeText;      // 时间段文本
    [SerializeField] private Image timeIcon;                // 时间图标

    [Header("地图名称")]
    [SerializeField] private TextMeshProUGUI mapNameText;

    [Header("精神值颜色配置")]
    [SerializeField] private Color sanityLevel0Color = Color.red;      // 0档：最危险
    [SerializeField] private Color sanityLevel1Color = new Color(1f, 0.3f, 0f); // 1档
    [SerializeField] private Color sanityLevel2Color = new Color(1f, 0.6f, 0f); // 2档
    [SerializeField] private Color sanityLevel3Color = Color.yellow;   // 3档：正常
    [SerializeField] private Color sanityLevel4Color = Color.green;    // 4档
    [SerializeField] private Color sanityLevel5Color = Color.cyan;     // 5档：最佳

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // 注册事件监听
        GameEvents.OnSanityChanged += OnSanityChanged;
        GameEvents.OnMoneyChanged += OnMoneyChanged;
        GameEvents.OnDayChanged += OnDayChanged;
        GameEvents.OnTimeSegmentChanged += OnTimeSegmentChanged;
    }

    private void OnDisable()
    {
        // 取消事件监听
        GameEvents.OnSanityChanged -= OnSanityChanged;
        GameEvents.OnMoneyChanged -= OnMoneyChanged;
        GameEvents.OnDayChanged -= OnDayChanged;
        GameEvents.OnTimeSegmentChanged -= OnTimeSegmentChanged;
    }

    private void Start()
    {
        // 初始化显示
        UpdateAllDisplays();
    }

    // ==================== 更新显示 ====================

    /// <summary>
    /// 更新所有显示
    /// </summary>
    public void UpdateAllDisplays()
    {
        if (GameDataManager.Instance == null) return;

        UpdateSanityDisplay(GameDataManager.Instance.Sanity);
        UpdateMoneyDisplay(GameDataManager.Instance.Money);
        UpdateDayDisplay(GameDataManager.Instance.CurrentDay);
        UpdateTimeSegmentDisplay(TimeManager.Instance != null ? TimeManager.Instance.CurrentSegment : TimeSegment.Morning_8_11);
    }

    // ==================== 精神值显示 ====================

    private void OnSanityChanged(int oldVal, int newVal)
    {
        UpdateSanityDisplay(newVal);
    }

    private void UpdateSanityDisplay(int sanity)
    {
        if (sanityIcons == null || sanityIcons.Length == 0) return;

        // 更新图标状态
        for (int i = 0; i < sanityIcons.Length; i++)
        {
            if (sanityIcons[i] == null) continue;

            bool isActive = i < sanity;
            sanityIcons[i].color = isActive ? sanityActiveColor : sanityInactiveColor;

            // 根据精神值等级切换图标
            if (isActive)
            {
                if (sanity <= 1 && sanityDangerSprite != null)
                    sanityIcons[i].sprite = sanityDangerSprite;
                else if (sanity <= 2 && sanityWarningSprite != null)
                    sanityIcons[i].sprite = sanityWarningSprite;
                else if (sanityNormalSprite != null)
                    sanityIcons[i].sprite = sanityNormalSprite;
            }
        }

        // 更新整体颜色提示（可选：背景色变化等）
        Color targetColor = GetSanityColor(sanity);
        // 这里可以添加额外的视觉反馈
    }

    private Color GetSanityColor(int sanity)
    {
        switch (sanity)
        {
            case 0: return sanityLevel0Color;
            case 1: return sanityLevel1Color;
            case 2: return sanityLevel2Color;
            case 3: return sanityLevel3Color;
            case 4: return sanityLevel4Color;
            case 5: return sanityLevel5Color;
            default: return Color.white;
        }
    }

    // ==================== 金钱显示 ====================

    private void OnMoneyChanged(int oldVal, int newVal)
    {
        UpdateMoneyDisplay(newVal);
    }

    private void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = string.Format(moneyFormat, money);
        }
    }

    // ==================== 时间显示 ====================

    private void OnDayChanged(int oldDay, int newDay)
    {
        UpdateDayDisplay(newDay);
    }

    private void OnTimeSegmentChanged(TimeSegment segment)
    {
        UpdateTimeSegmentDisplay(segment);
    }

    private void UpdateDayDisplay(int day)
    {
        if (dateText != null)
        {
            // 根据策划案，游戏从3月17日开始
            int startDay = 17 + day - 1;
            int month = 3;
            if (startDay > 31)
            {
                startDay -= 31;
                month = 4;
            }
            dateText.text = $"{month}月{startDay}日";
        }
    }

    private void UpdateTimeSegmentDisplay(TimeSegment segment)
    {
        if (timeText != null)
        {
            switch (segment)
            {
                case TimeSegment.Morning_8_11:
                    timeText.text = "8:00-11:00";
                    break;
                case TimeSegment.Noon_11_14:
                    timeText.text = "11:00-14:00";
                    break;
                case TimeSegment.Afternoon_14_17:
                    timeText.text = "14:00-17:00";
                    break;
                case TimeSegment.Dusk_17_20:
                    timeText.text = "17:00-20:00";
                    break;
                case TimeSegment.Night_20_23:
                    timeText.text = "20:00-23:00";
                    break;
            }
        }
    }

    // ==================== 地图名称 ====================

    /// <summary>
    /// 设置当前地图名称
    /// </summary>
    public void SetMapName(string mapName)
    {
        if (mapNameText != null)
        {
            mapNameText.text = mapName;
        }
    }
}
