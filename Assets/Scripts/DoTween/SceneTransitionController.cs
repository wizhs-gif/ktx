using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup blackOverlayGroup;
    [SerializeField] private CanvasGroup subtitleRootGroup;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI confirmText;

    [Header("Timing")]
    [SerializeField] private float fadeToBlackDuration = 0.45f;
    [SerializeField] private float subtitleFadeInDuration = 0.2f;
    [SerializeField] private float typeDuration = 1.8f;
    [SerializeField] private float fadeOutAfterLoadDuration = 0.8f;

    [Header("Input")]
    [SerializeField] private KeyCode confirmKey = KeyCode.Space;

    private Sequence currentSequence;
    private bool waitingForConfirm = false;
    private bool isLoading = false;
    private bool shouldFadeOutAfterLoad = false;
    private string targetSceneName;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        // 这个脚本必须挂在根物体上
        DontDestroyOnLoad(gameObject);

        blackOverlayGroup.alpha = 0f;
        blackOverlayGroup.blocksRaycasts = false;

        subtitleRootGroup.alpha = 0f;
        subtitleRootGroup.blocksRaycasts = false;

        subtitleText.text = "";
        subtitleText.maxVisibleCharacters = 0;

        confirmText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!waitingForConfirm || isLoading) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetKeyDown(confirmKey))
        {
            waitingForConfirm = false;
            confirmText.gameObject.SetActive(false);
            StartCoroutine(LoadNextSceneAsync());
        }
    }

    public void PlayTransitionAndLoad(string nextSceneName, string line)
    {
        if (IsPlaying || isLoading) return;
        if (string.IsNullOrWhiteSpace(nextSceneName)) return;

        IsPlaying = true;
        targetSceneName = nextSceneName;

        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();

        subtitleText.text = line;
        subtitleText.maxVisibleCharacters = 0;
        subtitleText.ForceMeshUpdate();

        int charCount = subtitleText.textInfo.characterCount;

        blackOverlayGroup.blocksRaycasts = true;
        subtitleRootGroup.blocksRaycasts = true;
        confirmText.gameObject.SetActive(false);

        currentSequence = DOTween.Sequence();
        currentSequence.Append(blackOverlayGroup.DOFade(1f, fadeToBlackDuration));
        currentSequence.Append(subtitleRootGroup.DOFade(1f, subtitleFadeInDuration));
        currentSequence.Append(
            DOTween.To(
                () => subtitleText.maxVisibleCharacters,
                x => subtitleText.maxVisibleCharacters = x,
                charCount,
                typeDuration
            ).SetEase(Ease.Linear)
        );
        currentSequence.AppendCallback(() =>
        {
            confirmText.gameObject.SetActive(true);
            waitingForConfirm = true;
        });
    }

    private IEnumerator LoadNextSceneAsync()
    {
        isLoading = true;
        shouldFadeOutAfterLoad = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);

        while (!op.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!shouldFadeOutAfterLoad) return;

        shouldFadeOutAfterLoad = false;
        StartCoroutine(FadeOutBlackAfterSceneLoad());
    }

    private IEnumerator FadeOutBlackAfterSceneLoad()
    {
        // 先隐藏字幕，只保留黑幕
        subtitleRootGroup.alpha = 0f;
        subtitleText.text = "";
        subtitleText.maxVisibleCharacters = 0;
        confirmText.gameObject.SetActive(false);

        // 确保黑幕当前是全黑
        blackOverlayGroup.alpha = 1f;

        yield return blackOverlayGroup
            .DOFade(0f, fadeOutAfterLoadDuration)
            .WaitForCompletion();

        blackOverlayGroup.blocksRaycasts = false;
        subtitleRootGroup.blocksRaycasts = false;

        IsPlaying = false;
        isLoading = false;
    }
}