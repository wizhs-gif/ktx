using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小游戏管理器 - 管理所有小游戏的启动和结果
/// </summary>
public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("小游戏引用")]
    [SerializeField] private GazeMiniGame gazeGame;           // 小游戏A：眺望
    [SerializeField] private DiceMiniGame diceGame;           // 小游戏C：摇骰子
    [SerializeField] private SlotMachineMiniGame slotGame;    // 小游戏D：老虎机
    [SerializeField] private MatchMiniGame matchGame;         // 小游戏E：连连看
    [SerializeField] private StockMiniGame stockGame;         // 小游戏F：股票
    [SerializeField] private SculptureMiniGame sculptureGame; // 小游戏B：雕塑交互

    private MiniGameBase currentGame;
    private Action<MiniGameResult> onGameComplete;

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

    /// <summary>
    /// 启动小游戏
    /// </summary>
    public void StartMiniGame(MiniGameType gameType, Action<MiniGameResult> callback)
    {
        onGameComplete = callback;

        switch (gameType)
        {
            case MiniGameType.Gaze:
                currentGame = gazeGame;
                break;
            case MiniGameType.Dice:
                currentGame = diceGame;
                break;
            case MiniGameType.SlotMachine:
                currentGame = slotGame;
                break;
            case MiniGameType.MatchGame:
                currentGame = matchGame;
                break;
            case MiniGameType.StockGame:
                currentGame = stockGame;
                break;
            case MiniGameType.SculptureGame:
                currentGame = sculptureGame;
                break;
            default:
                Debug.LogError($"[MiniGameManager] 未知的小游戏类型: {gameType}");
                return;
        }

        if (currentGame != null)
        {
            currentGame.StartMiniGame(OnMiniGameComplete);
        }
    }

    private void OnMiniGameComplete(bool success)
    {
        MiniGameResult result = new MiniGameResult(
            currentGame != null ? currentGame.name : "unknown",
            success,
            0f // 可以记录用时
        );

        onGameComplete?.Invoke(result);
        currentGame = null;
    }

    /// <summary>
    /// 检查小游戏是否正在运行
    /// </summary>
    public bool IsMiniGameRunning()
    {
        return currentGame != null;
    }
}

/// <summary>
/// 小游戏类型枚举
/// </summary>
public enum MiniGameType
{
    Gaze,           // 眺望（林翠公园）
    Dice,           // 摇骰子（医院）
    SlotMachine,    // 老虎机（广场）
    MatchGame,      // 连连看（办公室）
    StockGame,      // 股票（办公室）
    SculptureGame   // 雕塑交互（生命科学园）
}
