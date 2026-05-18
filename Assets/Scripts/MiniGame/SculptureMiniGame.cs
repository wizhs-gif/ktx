using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 雕塑交互小游戏 - 生命科学园
/// 规则：节奏按键，在正确时机按下按键，连续成功N次即可过关
/// </summary>
public class SculptureMiniGame : MiniGameBase
{
    [Header("节奏配置")]
    [SerializeField] private int requiredHits = 8;          // 需要成功的次数
    [SerializeField] private float beatInterval = 0.8f;     // 节拍间隔
    [SerializeField] private float hitWindow = 0.3f;        // 判定窗口（秒）
    [SerializeField] private int maxMisses = 3;             // 最大允许失误

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timingText;
    [SerializeField] private Image beatBar;
    [SerializeField] private Image hitZone;
    [SerializeField] private Image movingIndicator;
    [SerializeField] private Button actionButton;

    [Header("效果")]
    [SerializeField] private string rewardItemId = "medicine_robot";

    private int currentHits;
    private int currentMisses;
    private bool isBeatActive;
    private float beatTimer;
    private Coroutine beatCoroutine;

    // 节奏条相关
    private float barWidth;
    private float indicatorPos;
    private bool movingRight = true;
    private float moveSpeed;

    protected override void OnMiniGameStart()
    {
        currentHits = 0;
        currentMisses = 0;
        isBeatActive = false;

        // 计算移动速度
        if (beatBar != null)
        {
            barWidth = beatBar.GetComponent<RectTransform>().rect.width;
            moveSpeed = barWidth / beatInterval;
        }

        if (instructionText != null)
        {
            instructionText.text = "在标记到达高亮区域时按下空格键！";
        }

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnHitAttempt);
        }

        UpdateScoreText();
        beatCoroutine = StartCoroutine(BeatLoop());
    }

    private IEnumerator BeatLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (isPlaying)
        {
            // 重置指示器位置
            indicatorPos = 0f;
            movingRight = true;
            isBeatActive = true;

            if (timingText != null)
            {
                timingText.text = "准备...";
            }

            // 指示器从左到右移动
            while (isBeatActive && isPlaying)
            {
                float dt = Time.deltaTime;
                if (movingRight)
                {
                    indicatorPos += moveSpeed * dt;
                    if (indicatorPos >= barWidth)
                    {
                        indicatorPos = barWidth;
                        movingRight = false;
                    }
                }
                else
                {
                    indicatorPos -= moveSpeed * dt;
                    if (indicatorPos <= 0)
                    {
                        indicatorPos = 0;
                        movingRight = true;
                    }
                }

                // 更新指示器位置
                if (movingIndicator != null)
                {
                    RectTransform rt = movingIndicator.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchoredPosition = new Vector2(indicatorPos - barWidth / 2f, rt.anchoredPosition.y);
                    }
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Update()
    {
        if (!isPlaying) return;

        // 空格键也可以触发
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnHitAttempt();
        }

        // 更新时机提示
        if (isBeatActive && hitZone != null && movingIndicator != null)
        {
            float hitZoneCenter = hitZone.GetComponent<RectTransform>().anchoredPosition.x + barWidth / 2f;
            float distance = Mathf.Abs(indicatorPos - hitZoneCenter);
            float hitZoneHalfWidth = hitZone.GetComponent<RectTransform>().rect.width / 2f;

            if (distance < hitZoneHalfWidth)
            {
                if (timingText != null) timingText.text = "按下！";
            }
            else
            {
                if (timingText != null) timingText.text = "";
            }
        }
    }

    private void OnHitAttempt()
    {
        if (!isBeatActive) return;

        // 检查是否在判定窗口内
        if (hitZone == null || movingIndicator == null) return;

        float hitZoneCenter = hitZone.GetComponent<RectTransform>().anchoredPosition.x + barWidth / 2f;
        float distance = Mathf.Abs(indicatorPos - hitZoneCenter);
        float hitZoneHalfWidth = hitZone.GetComponent<RectTransform>().rect.width / 2f;

        if (distance <= hitZoneHalfWidth)
        {
            // 命中
            currentHits++;
            isBeatActive = false;

            if (timingText != null)
            {
                timingText.text = "命中！";
                timingText.color = Color.green;
            }

            UpdateScoreText();

            if (currentHits >= requiredHits)
            {
                // 通关
                if (GameDataManager.Instance != null && !string.IsNullOrEmpty(rewardItemId))
                {
                    GameDataManager.Instance.AddItem(rewardItemId);
                }

                if (instructionText != null)
                {
                    instructionText.text = "雕塑似乎回应了你的触碰...";
                }

                StartCoroutine(CloseAfterDelay(1.5f, true));
            }
        }
        else
        {
            // 失误
            currentMisses++;
            isBeatActive = false;

            if (timingText != null)
            {
                timingText.text = "失误！";
                timingText.color = Color.red;
            }

            UpdateScoreText();

            if (currentMisses >= maxMisses)
            {
                // 失败
                if (instructionText != null)
                {
                    instructionText.text = "雕塑没有反应...";
                }

                StartCoroutine(CloseAfterDelay(1f, false));
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"成功: {currentHits}/{requiredHits}  失误: {currentMisses}/{maxMisses}";
        }
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        // 节奏游戏不需要额外倒计时
    }

    private IEnumerator CloseAfterDelay(float delay, bool success)
    {
        yield return new WaitForSeconds(delay);
        EndMiniGame(success);
    }

    protected override void OnMiniGameEnd(bool success)
    {
        isBeatActive = false;
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
        }
    }
}
