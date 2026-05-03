using UnityEngine;
using System;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance { get; private set; }

    public TimeSegment CurrentSegment { get; private set; }
    
    // 关键接口：当时间段改变时触发，供光照系统、NPC系统监听
    public event Action<TimeSegment> OnTimeSegmentChanged; 

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // 每天开始时调用，重置为早晨
    public void InitializeDay() {
        SetTimeSegment(TimeSegment.Morning_8_11);
    }

    // 留给你的接口：切换场景、或触发某个主线事件后调用此方法推进时间
    public void AdvanceTime() {
        if (CurrentSegment == TimeSegment.Night_20_23) {
            // 如果已经是深夜，则触发“这一天结束”的逻辑
            Debug.Log("[TimeManager] 23点已到，今日结束。");
            TimelineManager.Instance.OnDayEnded();
        } else {
            // 否则进入下一个时间段
            SetTimeSegment(CurrentSegment + 1);
        }
    }

    // 也可以强制跳转到某个特定时间段
    public void SetTimeSegment(TimeSegment newSegment) {
        CurrentSegment = newSegment;
        Debug.Log($"[TimeManager] 当前时间已切换至: {CurrentSegment}");
        
        // 广播事件，通知所有监听者时间变了
        OnTimeSegmentChanged?.Invoke(CurrentSegment); 
    }
}