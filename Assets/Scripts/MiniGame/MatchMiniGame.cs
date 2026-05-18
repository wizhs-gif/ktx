using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 连连看小游戏 - 办公室"工作"选项
/// 规则：翻牌配对，找出所有相同图案的牌对
/// </summary>
public class MatchMiniGame : MiniGameBase
{
    [Header("连连看配置")]
    [SerializeField] private int gridRows = 4;
    [SerializeField] private int gridCols = 4;
    [SerializeField] private float flipBackDelay = 0.8f;

    [Header("UI")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI pairCountText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Sprite[] cardSprites;       // 不同图案的Sprite

    [Header("效果")]
    [SerializeField] private int rewardMoney = 60;

    private List<MatchCard> allCards = new List<MatchCard>();
    private MatchCard firstCard;
    private MatchCard secondCard;
    private bool isChecking;
    private int totalPairs;
    private int matchedPairs;

    protected override void OnMiniGameStart()
    {
        CreateCards();
        matchedPairs = 0;
        totalPairs = (gridRows * gridCols) / 2;

        UpdatePairCountText();
    }

    private void CreateCards()
    {
        // 清空
        foreach (var card in allCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        allCards.Clear();

        int totalCards = gridRows * gridCols;
        int pairCount = totalCards / 2;

        // 生成配对数据
        List<int> cardData = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            int spriteIndex = i % (cardSprites != null ? cardSprites.Length : 8);
            cardData.Add(spriteIndex);
            cardData.Add(spriteIndex);
        }

        // 洗牌
        for (int i = cardData.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cardData[i], cardData[j]) = (cardData[j], cardData[i]);
        }

        // 创建卡牌
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            MatchCard card = cardObj.GetComponent<MatchCard>();

            if (card != null)
            {
                Sprite faceSprite = (cardSprites != null && cardData[i] < cardSprites.Length)
                    ? cardSprites[cardData[i]] : null;
                card.Initialize(cardData[i], faceSprite);
                card.OnCardClicked += OnCardClicked;
                allCards.Add(card);
            }
        }
    }

    private void OnCardClicked(MatchCard card)
    {
        if (isChecking || card.IsMatched || card == firstCard) return;

        card.Flip(true);

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isChecking = true;

        yield return new WaitForSeconds(flipBackDelay);

        if (firstCard.CardId == secondCard.CardId)
        {
            // 配对成功
            firstCard.SetMatched();
            secondCard.SetMatched();
            matchedPairs++;

            UpdatePairCountText();

            if (matchedPairs >= totalPairs)
            {
                // 全部配对完成
                yield return new WaitForSeconds(0.5f);

                if (GameDataManager.Instance != null)
                {
                    GameDataManager.Instance.AddMoney(rewardMoney);
                }

                EndMiniGame(true);
            }
        }
        else
        {
            // 配对失败，翻回去
            firstCard.Flip(false);
            secondCard.Flip(false);
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    private void UpdatePairCountText()
    {
        if (pairCountText != null)
        {
            pairCountText.text = $"配对: {matchedPairs}/{totalPairs}";
        }
    }

    protected override void OnTimerUpdate(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"时间: {Mathf.CeilToInt(timeRemaining)}s";
        }
    }

    protected override void OnMiniGameEnd(bool success)
    {
        foreach (var card in allCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        allCards.Clear();
        firstCard = null;
        secondCard = null;
    }
}

/// <summary>
/// 连连看卡牌组件
/// </summary>
public class MatchCard : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image backImage;
    [SerializeField] private GameObject matchedIndicator;

    public event System.Action<MatchCard> OnCardClicked;

    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }

    private Sprite faceSprite;

    public void Initialize(int id, Sprite sprite)
    {
        CardId = id;
        faceSprite = sprite;
        IsMatched = false;
        IsFaceUp = false;

        if (cardImage != null && sprite != null)
        {
            cardImage.sprite = sprite;
        }

        Flip(false);
        if (matchedIndicator != null) matchedIndicator.SetActive(false);
    }

    public void Flip(bool faceUp)
    {
        IsFaceUp = faceUp;
        if (cardImage != null) cardImage.gameObject.SetActive(faceUp);
        if (backImage != null) backImage.gameObject.SetActive(!faceUp);
    }

    public void SetMatched()
    {
        IsMatched = true;
        if (matchedIndicator != null) matchedIndicator.SetActive(true);
    }

    public void OnClick()
    {
        if (!IsMatched && !IsFaceUp)
        {
            OnCardClicked?.Invoke(this);
        }
    }
}
