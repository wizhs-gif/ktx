using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 股票小游戏 - 办公室"看股票"选项
/// 规则：观察股价走势，在合适的时机买入卖出赚取差价
/// </summary>
public class StockMiniGame : MiniGameBase
{
    [Header("股票配置")]
    [SerializeField] private int startMoney = 100;          // 初始资金
    [SerializeField] private float priceUpdateInterval = 0.5f;
    [SerializeField] private float volatility = 0.15f;       // 波动率
    [SerializeField] private int targetProfit = 50;          // 目标盈利

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI stockNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI holdingText;
    [SerializeField] private TextMeshProUGUI profitText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button endButton;
    [SerializeField] private Transform chartContainer;
    [SerializeField] private GameObject chartDotPrefab;

    [Header("效果")]
    [SerializeField] private int failSanityChange = 1;

    private float currentPrice;
    private float basePrice;
    private int currentMoney;
    private int holdingShares;
    private int buyPrice;                     // 买入均价
    private List<float> priceHistory = new List<float>();
    private List<GameObject> chartDots = new List<GameObject>();
    private Coroutine priceUpdateCoroutine;

    private string[] stockNames = { "青庄科技", "五河生物", "通平医疗", "生命花园" };

    protected override void OnMiniGameStart()
    {
        basePrice = Random.Range(50f, 150f);
        currentPrice = basePrice;
        currentMoney = startMoney;
        holdingShares = 0;
        buyPrice = 0;
        priceHistory.Clear();

        // 清理图表
        foreach (var dot in chartDots)
        {
            if (dot != null) Destroy(dot);
        }
        chartDots.Clear();

        // 随机股票名
        if (stockNameText != null)
        {
            stockNameText.text = stockNames[Random.Range(0, stockNames.Length)];
        }

        // 绑定按钮
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnSellClicked);
        }
        if (endButton != null)
        {
            endButton.onClick.RemoveAllListeners();
            endButton.onClick.AddListener(OnEndClicked);
        }

        UpdateUI();
        priceUpdateCoroutine = StartCoroutine(PriceUpdateLoop());
    }

    private IEnumerator PriceUpdateLoop()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(priceUpdateInterval);
            UpdatePrice();
        }
    }

    private void UpdatePrice()
    {
        // 随机波动
        float change = Random.Range(-volatility, volatility);
        // 加一点趋势性
        float trend = Mathf.Sin(Time.time * 0.3f) * 0.02f;
        currentPrice *= (1f + change + trend);
        currentPrice = Mathf.Max(currentPrice, 1f);

        priceHistory.Add(currentPrice);

        // 更新图表
        UpdateChart();
        UpdateUI();
    }

    private void UpdateChart()
    {
        if (chartContainer == null || chartDotPrefab == null) return;

        // 限制显示点数
        int maxPoints = 40;
        int startIndex = Mathf.Max(0, priceHistory.Count - maxPoints);

        // 清理旧点
        foreach (var dot in chartDots)
        {
            if (dot != null) Destroy(dot);
        }
        chartDots.Clear();

        // 找出价格范围
        float minPrice = float.MaxValue;
        float maxPrice = float.MinValue;
        for (int i = startIndex; i < priceHistory.Count; i++)
        {
            minPrice = Mathf.Min(minPrice, priceHistory[i]);
            maxPrice = Mathf.Max(maxPrice, priceHistory[i]);
        }
        float priceRange = Mathf.Max(maxPrice - minPrice, 1f);

        // 绘制新点
        RectTransform chartRect = chartContainer.GetComponent<RectTransform>();
        float width = chartRect != null ? chartRect.rect.width : 400f;
        float height = chartRect != null ? chartRect.rect.height : 200f;

        int pointCount = priceHistory.Count - startIndex;
        for (int i = 0; i < pointCount; i++)
        {
            float x = (i / (float)Mathf.Max(maxPoints - 1, 1)) * width;
            float y = ((priceHistory[startIndex + i] - minPrice) / priceRange) * height;

            GameObject dot = Instantiate(chartDotPrefab, chartContainer);
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            if (dotRect != null)
            {
                dotRect.anchoredPosition = new Vector2(x, y);
            }

            // 涨跌颜色
            Image dotImage = dot.GetComponent<Image>();
            if (dotImage != null && i > 0)
            {
                dotImage.color = priceHistory[startIndex + i] >= priceHistory[startIndex + i - 1]
                    ? Color.green : Color.red;
            }

            chartDots.Add(dot);
        }
    }

    private void UpdateUI()
    {
        if (priceText != null)
            priceText.text = $"股价: ¥{currentPrice:F1}";

        if (moneyText != null)
            moneyText.text = $"资金: ¥{currentMoney}";

        if (holdingText != null)
            holdingText.text = $"持仓: {holdingShares}股";

        int totalValue = currentMoney + Mathf.RoundToInt(holdingShares * currentPrice);
        int profit = totalValue - startMoney;

        if (profitText != null)
        {
            profitText.text = $"盈亏: {(profit >= 0 ? "+" : "")}¥{profit}";
            profitText.color = profit >= 0 ? Color.green : Color.red;
        }

        // 按钮状态
        if (buyButton != null)
            buyButton.interactable = currentMoney >= Mathf.RoundToInt(currentPrice);
        if (sellButton != null)
            sellButton.interactable = holdingShares > 0;
    }

    private void OnBuyClicked()
    {
        int price = Mathf.RoundToInt(currentPrice);
        if (currentMoney < price) return;

        // 买入1股
        int shares = Mathf.FloorToInt(currentMoney / price);
        shares = Mathf.Max(shares, 1);

        int cost = shares * price;
        currentMoney -= cost;

        // 更新买入均价
        if (holdingShares == 0)
        {
            buyPrice = price;
        }
        else
        {
            buyPrice = (buyPrice * holdingShares + cost) / (holdingShares + shares);
        }

        holdingShares += shares;

        ShowMessage($"买入 {shares}股 @ ¥{price}");
        UpdateUI();
    }

    private void OnSellClicked()
    {
        if (holdingShares <= 0) return;

        int price = Mathf.RoundToInt(currentPrice);
        int income = holdingShares * price;
        currentMoney += income;

        ShowMessage($"卖出 {holdingShares}股 @ ¥{price}");
        holdingShares = 0;
        UpdateUI();
    }

    private void OnEndClicked()
    {
        // 卖出所有持仓
        if (holdingShares > 0)
        {
            currentMoney += Mathf.RoundToInt(holdingShares * currentPrice);
            holdingShares = 0;
        }

        int profit = currentMoney - startMoney;
        bool success = profit >= targetProfit;

        if (GameDataManager.Instance != null)
        {
            if (success)
            {
                GameDataManager.Instance.AddMoney(profit);
            }
            else
            {
                GameDataManager.Instance.AddSanity(failSanityChange);
            }
        }

        if (messageText != null)
        {
            messageText.text = success ?
                $"盈利 ¥{profit}！投资成功！" :
                $"亏损 ¥{Mathf.Abs(profit)}...";
        }

        StartCoroutine(CloseAfterDelay(1.5f));
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
        }
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bool success = (currentMoney - startMoney) >= targetProfit;
        EndMiniGame(success);
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        // 可选：显示剩余时间
    }

    protected override void OnMiniGameEnd(bool success)
    {
        if (priceUpdateCoroutine != null)
        {
            StopCoroutine(priceUpdateCoroutine);
        }

        foreach (var dot in chartDots)
        {
            if (dot != null) Destroy(dot);
        }
        chartDots.Clear();
    }
}
