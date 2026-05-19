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
    [SerializeField] private Image[] sanityBorders;        // 屏幕边缘遮罩（6个，从外到内）
    [SerializeField] private NoiseOverlay noiseOverlay;    // 噪点覆盖层

    [Header("金钱显示")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string moneyFormat = "¥{0}";

    [Header("时间显示")]
    [SerializeField] private TextMeshProUGUI dateText;      // 日期文本
    [SerializeField] private TextMeshProUGUI timeText;      // 时间段文本
    [SerializeField] private Image timeIcon;                // 时间图标

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
        // 更新屏幕边缘遮罩：element0 始终显示，精神值越低遮罩越多
        // sanity=5 → 只有 element0, sanity=0 → 6个全满
        if (sanityBorders != null)
        {
            for (int i = 0; i < sanityBorders.Length; i++)
            {
                if (sanityBorders[i] == null) continue;

                // element0 始终显示，其余随精神值降低逐个出现
                // element[i] 在 sanity <= (SANITY_MAX - i) 时显示
                bool shouldShow = i == 0 || sanity <= (GameDataManager.SANITY_MAX - i);

                Color c = sanityBorders[i].color;
                c.a = shouldShow ? 1f : 0f;
                sanityBorders[i].color = c;
            }
        }

        // 更新噪点覆盖：精神值越低，噪点越明显
        if (noiseOverlay != null)
        {
            float t = 1f - (float)sanity / GameDataManager.SANITY_MAX; // 0→1, sanity越高t越低
            noiseOverlay.SetIntensity(t);
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

}
