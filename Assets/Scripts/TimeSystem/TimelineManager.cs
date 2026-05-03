using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour {
    public static TimelineManager Instance { get; private set; }

    [Header("时间线状态")]
    public int currentWorldDay = 5; // 假设游戏从第 5 天（最后一天）开始
    public List<int> unlockedDays = new List<int>() { 5 }; // 已解锁的天数

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景时不销毁时间线管理器
        } else {
            Destroy(gameObject);
        }
    }

    // 这一天过完时触发
    public void OnDayEnded() {
        Debug.Log($"[TimelineManager] 第 {currentWorldDay} 天已结束，进入时间夹缝。");
        // TODO: 这里可以呼出 UI 面板，让玩家选择下一步。
        // UIManager.Instance.ShowTimelineSelectionPanel();
    }

    // ================= UI 按钮绑定的 3 个核心功能 =================

    // 1. 重过一遍本天
    public void ReplayCurrentDay() {
        Debug.Log($"[TimelineManager] 重启轮回：重新开始第 {currentWorldDay} 天。");
        // TODO: 清理这天的临时存档（如果有的话）
        LoadDay(currentWorldDay);
    }

    // 2. 往前一天 (推演过去，数字减小，比如从 Day 5 回到 Day 4)
    public void GoToPreviousDay() {
        int previousDay = currentWorldDay - 1; 
        
        if (previousDay >= 1) { // 假设第 1 天是故事的起点
            if (!unlockedDays.Contains(previousDay)) {
                unlockedDays.Add(previousDay); // 解锁新的一天
            }
            Debug.Log($"[TimelineManager] 溯洄时间：前往第 {previousDay} 天。");
            LoadDay(previousDay);
        } else {
            Debug.Log("[TimelineManager] 已经是故事的最开头，无法再往前推演了！");
        }
    }

    /*// 3. 往后一天 (回到未来，数字增大，比如从 Day 4 去看 Day 5 发生的改变)
    public void GoToNextDay() {
        int nextDay = currentWorldDay + 1;
        
        if (unlockedDays.Contains(nextDay)) {
            Debug.Log($"[TimelineManager] 蝴蝶效应：前往被改变的第 {nextDay} 天。");
            // TODO: 这里可以执行“历史覆写”的存档逻辑
            LoadDay(nextDay);
        } else {
            Debug.Log("[TimelineManager] 未来尚未解锁！");
        }
    }*/

    // ================= 核心流转逻辑 =================

    private void LoadDay(int targetDay) {
        currentWorldDay = targetDay;
        
        // 1. 读取那一天对应的存档数据
        // SaveManager.Instance.LoadDayData(targetDay);
        
        // 2. 重新加载场景 (比如回到主角的出租屋，这是每天开始的固定点)
        // SceneManager.LoadScene("PlayerApartmentScene");
        
        // 3. 将单日时间重置为早晨
        if (TimeManager.Instance != null) {
            TimeManager.Instance.InitializeDay();
        }
    }
}