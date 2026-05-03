using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 眺望小游戏 - 林翠公园观景台
/// 规则：限时完成，成功精神-1，失败精神+1
/// </summary>
public class GazeMiniGame : MiniGameBase
{
    [Header("眺望游戏配置")]
    [SerializeField] private float gazeTimeRequired = 5f;  // 需要眺望的时间
    [SerializeField] private float gazeRange = 0.8f;       // 眺望判定范围（0-1）

    [Header("UI")]
    [SerializeField] private Image gazeProgressBar;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private RectTransform gazeTarget;     // 眺望目标区域
    [SerializeField] private RectTransform gazeCursor;     // 眺望光标

    [Header("效果")]
    [SerializeField] private int successSanityChange = -1;
    [SerializeField] private int failSanityChange = 1;

    private float gazeProgress;
    private bool isGazing;
    private Camera mainCamera;

    protected override void OnMiniGameStart()
    {
        gazeProgress = 0f;
        isGazing = false;
        mainCamera = Camera.main;

        if (gazeProgressBar != null)
        {
            gazeProgressBar.fillAmount = 0f;
        }

        if (instructionText != null)
        {
            instructionText.text = "将视线保持在目标区域内";
        }
    }

    protected override void OnMiniGameEnd(bool success)
    {
        // 应用效果
        if (GameDataManager.Instance != null)
        {
            if (success)
            {
                GameDataManager.Instance.AddSanity(successSanityChange);
            }
            else
            {
                GameDataManager.Instance.AddSanity(failSanityChange);
            }
        }
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        // 检查眺望位置
        CheckGazePosition();

        // 更新进度
        if (isGazing)
        {
            gazeProgress += Time.deltaTime;
        }
        else
        {
            gazeProgress -= Time.deltaTime * 0.5f; // 退步速度较慢
        }

        gazeProgress = Mathf.Clamp(gazeProgress, 0f, gazeTimeRequired);

        // 更新UI
        if (gazeProgressBar != null)
        {
            gazeProgressBar.fillAmount = gazeProgress / gazeTimeRequired;
        }

        // 检查是否完成
        if (gazeProgress >= gazeTimeRequired)
        {
            EndMiniGame(true);
        }
    }

    private void CheckGazePosition()
    {
        if (mainCamera == null || gazeTarget == null) return;

        // 获取屏幕中心点
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // 检查是否在目标区域内
        Vector2 targetPos = gazeTarget.position;
        float distance = Vector2.Distance(screenCenter, targetPos);
        float maxDistance = Screen.width * gazeRange / 2f;

        isGazing = distance <= maxDistance;

        // 更新光标位置
        if (gazeCursor != null)
        {
            gazeCursor.position = screenCenter;
            gazeCursor.gameObject.SetActive(isGazing);
        }
    }

    protected override void OnTimeUp()
    {
        // 时间到，根据当前进度判断
        bool success = gazeProgress >= gazeTimeRequired * 0.8f; // 80%以上算成功
        EndMiniGame(success);
    }
}
