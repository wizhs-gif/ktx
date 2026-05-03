using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏流程管理器 - 管理游戏的整体流程和状态
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("UI引用")]
    [SerializeField] private TimeSelectionUI timeSelectionUI;
    [SerializeField] private DayEndUI dayEndUI;

    private bool isDayActive;
    private List<TimeSegment> selectedTimeSegments;
    private int currentTimeSegmentIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnDayStarted += OnDayStarted;
        GameEvents.OnDayEnded += OnDayEnded;
    }

    private void OnDisable()
    {
        GameEvents.OnDayStarted -= OnDayStarted;
        GameEvents.OnDayEnded -= OnDayEnded;
    }

    /// <summary>
    /// 开始新的一天
    /// </summary>
    public void StartDay()
    {
        if (GameDataManager.Instance == null) return;

        int currentDay = GameDataManager.Instance.CurrentDay;
        Debug.Log($"[GameFlowManager] 开始第{currentDay}天");

        // 触发每日开始事件
        GameEvents.DayStarted(currentDay);

        // 显示时间选择UI
        if (timeSelectionUI != null)
        {
            timeSelectionUI.OpenSelection(OnTimeSelectionComplete);
        }
        else
        {
            // 如果没有时间选择UI，直接开始
            StartDayWithSegments(new List<TimeSegment>
            {
                TimeSegment.Morning_8_11,
                TimeSegment.Noon_11_14,
                TimeSegment.Afternoon_14_17,
                TimeSegment.Dusk_17_20
            });
        }
    }

    private void OnTimeSelectionComplete(List<TimeSegment> segments)
    {
        StartDayWithSegments(segments);
    }

    private void StartDayWithSegments(List<TimeSegment> segments)
    {
        selectedTimeSegments = segments;
        currentTimeSegmentIndex = 0;
        isDayActive = true;

        // 开始第一个时间段
        if (selectedTimeSegments.Count > 0)
        {
            AdvanceToNextTimeSegment();
        }
    }

    /// <summary>
    /// 推进到下一个时间段
    /// </summary>
    public void AdvanceToNextTimeSegment()
    {
        if (!isDayActive) return;

        currentTimeSegmentIndex++;

        if (currentTimeSegmentIndex >= selectedTimeSegments.Count)
        {
            // 所有时间段结束，结束这一天
            EndDay();
        }
        else
        {
            // 切换到下一个时间段
            TimeSegment nextSegment = selectedTimeSegments[currentTimeSegmentIndex];

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTimeSegment(nextSegment);
            }

            Debug.Log($"[GameFlowManager] 进入时间段: {nextSegment}");
        }
    }

    /// <summary>
    /// 结束这一天
    /// </summary>
    public void EndDay()
    {
        isDayActive = false;

        if (GameDataManager.Instance == null) return;

        int currentDay = GameDataManager.Instance.CurrentDay;
        Debug.Log($"[GameFlowManager] 第{currentDay}天结束");

        // 触发每日结束事件
        GameEvents.DayEnded(currentDay);

        // 显示每日结束UI
        if (dayEndUI != null)
        {
            dayEndUI.ShowDayEnd(OnDayEndChoice);
        }
    }

    private void OnDayEndChoice(DayEndChoice choice)
    {
        switch (choice)
        {
            case DayEndChoice.NextDay:
                // 前往明天
                if (GameDataManager.Instance != null)
                {
                    GameDataManager.Instance.AdvanceToNextDay();
                }
                StartDay();
                break;

            case DayEndChoice.ReplayDay:
                // 重开今天
                if (GameDataManager.Instance != null)
                {
                    GameDataManager.Instance.ResetDayData();
                }
                StartDay();
                break;

            case DayEndChoice.PreviousDay:
                // 回到昨天
                if (GameDataManager.Instance != null)
                {
                    GameDataManager.Instance.GoToPreviousDay();
                }
                StartDay();
                break;
        }
    }

    private void OnDayStarted(int day)
    {
        Debug.Log($"[GameFlowManager] 第{day}天开始");
    }

    private void OnDayEnded(int day)
    {
        Debug.Log($"[GameFlowManager] 第{day}天结束");
    }

    /// <summary>
    /// 检查是否在活动时间段内
    /// </summary>
    public bool IsActiveTimeSegment()
    {
        return isDayActive;
    }

    /// <summary>
    /// 获取当前时间段
    /// </summary>
    public TimeSegment GetCurrentTimeSegment()
    {
        if (selectedTimeSegments == null || currentTimeSegmentIndex >= selectedTimeSegments.Count)
        {
            return TimeSegment.Morning_8_11;
        }

        return selectedTimeSegments[currentTimeSegmentIndex];
    }
}
