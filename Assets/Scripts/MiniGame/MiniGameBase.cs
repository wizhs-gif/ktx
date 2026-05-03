using System;
using UnityEngine;

/// <summary>
/// 小游戏基类 - 所有小游戏继承此类
/// </summary>
public abstract class MiniGameBase : MonoBehaviour
{
    [Header("小游戏配置")]
    [SerializeField] protected string miniGameId;
    [SerializeField] protected string miniGameName;
    [SerializeField] protected float timeLimit = 10f;

    [Header("UI")]
    [SerializeField] protected GameObject miniGamePanel;
    [SerializeField] protected UnityEngine.UI.Button closeButton;

    protected bool isPlaying;
    protected float currentTime;
    protected Action<bool> onComplete; // true=成功, false=失败

    protected virtual void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CancelMiniGame);
        }
    }

    protected virtual void Update()
    {
        if (!isPlaying) return;

        currentTime -= Time.deltaTime;
        OnTimerUpdate(currentTime);

        if (currentTime <= 0)
        {
            OnTimeUp();
        }
    }

    /// <summary>
    /// 开始小游戏
    /// </summary>
    public virtual void StartMiniGame(Action<bool> callback)
    {
        onComplete = callback;
        isPlaying = true;
        currentTime = timeLimit;

        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(true);
        }

        OnMiniGameStart();
        Debug.Log($"[MiniGame] 开始小游戏: {miniGameName}");
    }

    /// <summary>
    /// 结束小游戏
    /// </summary>
    protected virtual void EndMiniGame(bool success)
    {
        isPlaying = false;

        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(false);
        }

        OnMiniGameEnd(success);
        onComplete?.Invoke(success);

        Debug.Log($"[MiniGame] 小游戏结束: {miniGameName}, 结果: {(success ? "成功" : "失败")}");
    }

    /// <summary>
    /// 取消小游戏
    /// </summary>
    protected virtual void CancelMiniGame()
    {
        EndMiniGame(false);
    }

    /// <summary>
    /// 时间到
    /// </summary>
    protected virtual void OnTimeUp()
    {
        EndMiniGame(false);
    }

    // 子类需要实现的方法
    protected abstract void OnMiniGameStart();
    protected abstract void OnMiniGameEnd(bool success);
    protected abstract void OnTimerUpdate(float timeRemaining);
}

/// <summary>
/// 小游戏结果数据
/// </summary>
public class MiniGameResult
{
    public string miniGameId;
    public bool success;
    public float timeUsed;
    public int score;

    public MiniGameResult(string id, bool success, float timeUsed, int score = 0)
    {
        this.miniGameId = id;
        this.success = success;
        this.timeUsed = timeUsed;
        this.score = score;
    }
}
