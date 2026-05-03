using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 摇骰子小游戏 - 医院交互
/// 规则：5个骰子选3个，检定大小
/// </summary>
public class DiceMiniGame : MiniGameBase
{
    [Header("骰子配置")]
    [SerializeField] private int totalDice = 5;
    [SerializeField] private int selectCount = 3;
    [SerializeField] private int targetSum = 10;    // 目标点数

    [Header("UI")]
    [SerializeField] private Transform diceContainer;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button rollButton;
    [SerializeField] private Button confirmButton;

    [Header("效果")]
    [SerializeField] private int successSanityChange = 0;
    [SerializeField] private int failSanityChange = 1;

    private List<Dice> diceList = new List<Dice>();
    private List<Dice> selectedDice = new List<Dice>();
    private bool isRolling;
    private bool hasRolled;

    protected override void OnMiniGameStart()
    {
        // 创建骰子
        CreateDice();

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(OnRollClicked);
            rollButton.interactable = true;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            confirmButton.interactable = false;
        }

        if (instructionText != null)
        {
            instructionText.text = $"选择{selectCount}个骰子，点数之和需达到{targetSum}";
        }

        hasRolled = false;
    }

    private void CreateDice()
    {
        // 清空现有骰子
        foreach (var dice in diceList)
        {
            if (dice != null) Destroy(dice.gameObject);
        }
        diceList.Clear();
        selectedDice.Clear();

        // 创建新骰子
        for (int i = 0; i < totalDice; i++)
        {
            GameObject diceObj = Instantiate(dicePrefab, diceContainer);
            Dice dice = diceObj.GetComponent<Dice>();

            if (dice != null)
            {
                dice.Initialize(i);
                dice.OnDiceClicked += OnDiceClicked;
                diceList.Add(dice);
            }
        }
    }

    private void OnRollClicked()
    {
        if (hasRolled) return;

        StartCoroutine(RollAllDice());
    }

    private IEnumerator RollAllDice()
    {
        isRolling = true;
        hasRolled = true;

        if (rollButton != null) rollButton.interactable = false;

        // 摇动动画
        foreach (var dice in diceList)
        {
            dice.StartRolling();
        }

        yield return new WaitForSeconds(1.5f);

        // 停止摇动，显示结果
        foreach (var dice in diceList)
        {
            dice.StopRolling();
        }

        isRolling = false;

        if (confirmButton != null) confirmButton.interactable = true;
        if (instructionText != null) instructionText.text = "选择3个骰子";
    }

    private void OnDiceClicked(Dice dice)
    {
        if (isRolling) return;

        if (selectedDice.Contains(dice))
        {
            // 取消选择
            selectedDice.Remove(dice);
            dice.SetSelected(false);
        }
        else if (selectedDice.Count < selectCount)
        {
            // 选择
            selectedDice.Add(dice);
            dice.SetSelected(true);
        }

        // 更新确认按钮
        if (confirmButton != null)
        {
            confirmButton.interactable = selectedDice.Count == selectCount;
        }

        // 更新提示
        if (instructionText != null)
        {
            instructionText.text = $"已选择 {selectedDice.Count}/{selectCount} 个骰子";
        }
    }

    private void OnConfirmClicked()
    {
        if (selectedDice.Count != selectCount) return;

        // 计算点数之和
        int sum = 0;
        foreach (var dice in selectedDice)
        {
            sum += dice.Value;
        }

        // 判断成功/失败
        bool success = sum >= targetSum;

        // 显示结果
        if (resultText != null)
        {
            resultText.text = $"点数之和: {sum}\n{(success ? "成功!" : "失败...")}";
        }

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

        // 延迟关闭
        StartCoroutine(CloseAfterDelay(1.5f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndMiniGame(selectedDice.Count == selectCount && GetSum() >= targetSum);
    }

    private int GetSum()
    {
        int sum = 0;
        foreach (var dice in selectedDice)
        {
            sum += dice.Value;
        }
        return sum;
    }

    protected override void OnMiniGameEnd(bool success)
    {
        // 清理
        foreach (var dice in diceList)
        {
            if (dice != null) Destroy(dice.gameObject);
        }
        diceList.Clear();
        selectedDice.Clear();
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        // 可选：显示倒计时
    }
}

/// <summary>
/// 骰子组件
/// </summary>
public class Dice : MonoBehaviour
{
    [SerializeField] private Image diceImage;
    [SerializeField] private Sprite[] diceSprites; // 1-6的骰子图片
    [SerializeField] private Image selectionHighlight;

    public event System.Action<Dice> OnDiceClicked;

    public int Value { get; private set; }
    public bool IsSelected { get; private set; }

    private bool isRolling;
    private int diceIndex;

    public void Initialize(int index)
    {
        diceIndex = index;
        Value = Random.Range(1, 7);
        UpdateDisplay();
    }

    public void StartRolling()
    {
        isRolling = true;
        StartCoroutine(RollAnimation());
    }

    public void StopRolling()
    {
        isRolling = false;
        Value = Random.Range(1, 7);
        UpdateDisplay();
    }

    private IEnumerator RollAnimation()
    {
        while (isRolling)
        {
            int randomValue = Random.Range(1, 7);
            if (diceSprites != null && randomValue <= diceSprites.Length)
            {
                diceImage.sprite = diceSprites[randomValue - 1];
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        if (selectionHighlight != null)
        {
            selectionHighlight.gameObject.SetActive(selected);
        }
    }

    private void UpdateDisplay()
    {
        if (diceImage != null && diceSprites != null && Value >= 1 && Value <= diceSprites.Length)
        {
            diceImage.sprite = diceSprites[Value - 1];
        }
    }

    public void OnClick()
    {
        if (!isRolling)
        {
            OnDiceClicked?.Invoke(this);
        }
    }
}
