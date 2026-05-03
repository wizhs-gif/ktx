using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 老虎机小游戏 - 广场彩票店
/// 规则：六位数字老虎机，匹配中奖
/// </summary>
public class SlotMachineMiniGame : MiniGameBase
{
    [Header("老虎机配置")]
    [SerializeField] private int digitCount = 6;
    [SerializeField] private int[] prizeAmounts = { 100, 50, 20, 10 }; // 奖金等级
    [SerializeField] private int[] matchCounts = { 6, 5, 4, 3 };      // 匹配数量

    [Header("UI")]
    [SerializeField] private Transform digitContainer;
    [SerializeField] private GameObject digitPrefab;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI prizeText;
    [SerializeField] private Button spinButton;

    [Header("动画")]
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float spinInterval = 0.1f;

    private List<SlotDigit> digits = new List<SlotDigit>();
    private List<int> targetNumbers = new List<int>();
    private bool isSpinning;

    protected override void OnMiniGameStart()
    {
        // 创建数字滚轮
        CreateDigits();

        // 生成目标数字
        GenerateTargetNumbers();

        if (spinButton != null)
        {
            spinButton.onClick.AddListener(OnSpinClicked);
            spinButton.interactable = true;
        }

        if (resultText != null)
        {
            resultText.text = "点击开始";
        }
    }

    private void CreateDigits()
    {
        // 清空现有
        foreach (var digit in digits)
        {
            if (digit != null) Destroy(digit.gameObject);
        }
        digits.Clear();

        // 创建新的
        for (int i = 0; i < digitCount; i++)
        {
            GameObject digitObj = Instantiate(digitPrefab, digitContainer);
            SlotDigit digit = digitObj.GetComponent<SlotDigit>();

            if (digit != null)
            {
                digit.Initialize(i);
                digits.Add(digit);
            }
        }
    }

    private void GenerateTargetNumbers()
    {
        targetNumbers.Clear();
        for (int i = 0; i < digitCount; i++)
        {
            targetNumbers.Add(Random.Range(0, 10));
        }
    }

    private void OnSpinClicked()
    {
        if (isSpinning) return;
        StartCoroutine(SpinAnimation());
    }

    private IEnumerator SpinAnimation()
    {
        isSpinning = true;

        if (spinButton != null) spinButton.interactable = false;
        if (resultText != null) resultText.text = "转动中...";

        // 开始所有滚轮转动
        foreach (var digit in digits)
        {
            digit.StartSpinning();
        }

        // 逐个停止
        for (int i = 0; i < digits.Count; i++)
        {
            yield return new WaitForSeconds(spinDuration / digitCount);
            digits[i].StopSpinning(targetNumbers[i]);
        }

        yield return new WaitForSeconds(0.5f);

        // 计算结果
        int prize = CalculatePrize();

        if (prizeText != null)
        {
            prizeText.text = prize > 0 ? $"中奖! 奖金: ¥{prize}" : "未中奖";
        }

        // 应用奖金
        if (prize > 0 && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.AddMoney(prize);
        }

        isSpinning = false;

        // 延迟关闭
        yield return new WaitForSeconds(1.5f);
        EndMiniGame(prize > 0);
    }

    private int CalculatePrize()
    {
        // 统计每个数字出现的次数
        Dictionary<int, int> digitCounts = new Dictionary<int, int>();
        foreach (var digit in digits)
        {
            if (!digitCounts.ContainsKey(digit.Value))
            {
                digitCounts[digit.Value] = 0;
            }
            digitCounts[digit.Value]++;
        }

        // 找到最大匹配数
        int maxMatch = 0;
        foreach (var count in digitCounts.Values)
        {
            if (count > maxMatch) maxMatch = count;
        }

        // 计算奖金
        for (int i = 0; i < matchCounts.Length; i++)
        {
            if (maxMatch >= matchCounts[i])
            {
                return prizeAmounts[i];
            }
        }

        return 0;
    }

    protected override void OnMiniGameEnd(bool success)
    {
        // 清理
        foreach (var digit in digits)
        {
            if (digit != null) Destroy(digit.gameObject);
        }
        digits.Clear();
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        // 可选：显示倒计时
    }
}

/// <summary>
/// 数字滚轮组件
/// </summary>
public class SlotDigit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI digitText;
    [SerializeField] private Image digitBackground;

    public int Value { get; private set; }
    private bool isSpinning;
    private int digitIndex;

    public void Initialize(int index)
    {
        digitIndex = index;
        Value = Random.Range(0, 10);
        UpdateDisplay();
    }

    public void StartSpinning()
    {
        isSpinning = true;
        StartCoroutine(SpinDigit());
    }

    public void StopSpinning(int finalValue)
    {
        isSpinning = false;
        Value = finalValue;
        UpdateDisplay();
    }

    private IEnumerator SpinDigit()
    {
        while (isSpinning)
        {
            Value = Random.Range(0, 10);
            UpdateDisplay();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void UpdateDisplay()
    {
        if (digitText != null)
        {
            digitText.text = Value.ToString();
        }
    }
}
