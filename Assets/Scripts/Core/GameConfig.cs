using UnityEngine;

/// <summary>
/// 游戏配置 - 存储各种游戏参数配置
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("上班工资")]
    public int normalWorkSalary = 30;      // 正常上班工资
    public int activeWorkSalary = 60;      // 选择工作时的工资
    public int coffeeShopSalary = 20;      // 咖啡馆打工工资

    [Header("精神值影响")]
    public int sanityLossFromWork = 0;     // 工作失去的精神值
    public int sanityGainFromRest = -2;    // 休息恢复的精神值（负数表示恢复）
    public int sanityGainFromThink = 1;    // 思考增加的精神值

    [Header("小游戏配置")]
    public float gazeGameTimeLimit = 10f;  // 眺望小游戏时间限制（秒）
    public int diceTotalCount = 5;         // 骰子总数
    public int diceSelectCount = 3;        // 选择骰子数量
    public int lotteryDigitCount = 6;      // 彩票数字位数

    [Header("交互配置")]
    public float interactRange = 2f;       // 交互触发距离
    public KeyCode interactKey = KeyCode.E; // 交互按键

    [Header("上班时段选择")]
    public float timeSelectCountdown = 8f; // 选择倒计时（秒）
    public int minTimeSlots = 2;           // 最少选择时间段数
    public int maxTimeSlots = 4;           // 最多选择时间段数

    [Header("咖啡效果")]
    public int coffee1SanityChange = 1;    // 咖啡1精神值变化
    public int coffee2SanityChange = -1;   // 咖啡2精神值变化
    public int coffee3MoneyBonus = 10;     // 咖啡3挣钱效率加成
    public float coffee4SpeedBonus = 1.2f; // 咖啡4移动速度加成
}
