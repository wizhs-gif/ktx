using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlackSubtitleTransition : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup blackGroup;
    [SerializeField] private CanvasGroup subtitleGroup;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Timing")]
    [SerializeField] private float fadeToBlackDuration = 0.45f;
    [SerializeField] private float fadeSubtitleInDuration = 0.2f;
    [SerializeField] private float typeDuration = 1.8f;
    [SerializeField] private float holdDuration = 0.8f;
    [SerializeField] private float fadeOutDuration = 0.35f;

    [Header("Input")]
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    private Sequence currentSequence;
    private bool canSkipTyping;

    private void Awake()
    {
        blackGroup.alpha = 0f;
        blackGroup.blocksRaycasts = false;

        subtitleGroup.alpha = 0f;
        subtitleGroup.blocksRaycasts = false;

        subtitleText.text = "";
        subtitleText.maxVisibleCharacters = 0;
    }

    private void Update()
    {
        if (canSkipTyping && Input.GetKeyDown(skipKey))
        {
            subtitleText.maxVisibleCharacters = int.MaxValue;
            canSkipTyping = false;
        }
    }

    public void PlayLine(string line)
    {
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();

        canSkipTyping = false;

        subtitleText.text = line;
        subtitleText.maxVisibleCharacters = 0;

        blackGroup.blocksRaycasts = true;
        subtitleGroup.blocksRaycasts = true;

        // 很重要：让 TMP 先生成字符信息
        subtitleText.ForceMeshUpdate();

        int visibleCount = subtitleText.textInfo.characterCount;

        currentSequence = DOTween.Sequence();

        currentSequence.Append(blackGroup.DOFade(1f, fadeToBlackDuration));
        currentSequence.AppendCallback(() =>
        {
            subtitleGroup.alpha = 0f;
            subtitleText.maxVisibleCharacters = 0;
        });

        currentSequence.Append(subtitleGroup.DOFade(1f, fadeSubtitleInDuration));

        currentSequence.AppendCallback(() =>
        {
            canSkipTyping = true;
        });

        currentSequence.Append(
            DOTween.To(
                () => subtitleText.maxVisibleCharacters,
                x => subtitleText.maxVisibleCharacters = x,
                visibleCount,
                typeDuration
            ).SetEase(Ease.Linear)
        );

        currentSequence.AppendCallback(() =>
        {
            canSkipTyping = false;
        });

        currentSequence.AppendInterval(holdDuration);
        currentSequence.Append(subtitleGroup.DOFade(0f, fadeOutDuration));
        currentSequence.Append(blackGroup.DOFade(0f, fadeOutDuration));

        currentSequence.OnComplete(() =>
        {
            blackGroup.blocksRaycasts = false;
            subtitleGroup.blocksRaycasts = false;
            subtitleText.text = "";
            subtitleText.maxVisibleCharacters = 0;
        });
    }
}