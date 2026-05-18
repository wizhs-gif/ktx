using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 开场剧情序列 - 实现策划案中的5天开场
/// 挂载在 Home 场景中
/// </summary>
public class OpeningSequence : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private Image blackoutImage;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private GameObject subwayUIRoot;

    [Header("开场序列配置")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float subtitleDisplayTime = 2f;
    [SerializeField] private float typewriterSpeed = 0.05f;

    [Header("眩晕特效")]
    [SerializeField] private Image vignetteOverlay;
    [SerializeField] private float maxVignetteAlpha = 0.6f;

    [Header("虚化特效")]
    [SerializeField] private Image blurOverlay;
    [SerializeField] private float blurFadeDuration = 2f;

    [Header("交互提示")]
    [SerializeField] private GameObject interactPromptRoot;
    [SerializeField] private GameObject subwayMapObject;    // 墙上地铁线路图
    [SerializeField] private GameObject bookshelfObject;    // 书架
    [SerializeField] private GameObject coffeeMachineObject;// 咖啡机

    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        // 检查是否已完成开场（避免重复播放）
        if (GameDataManager.Instance != null && GameDataManager.Instance.CurrentDay > 1)
        {
            SkipOpening();
            return;
        }

        StartCoroutine(PlayOpeningSequence());
    }

    private void SkipOpening()
    {
        if (blackoutImage != null) blackoutImage.gameObject.SetActive(false);
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);
        if (vignetteOverlay != null) vignetteOverlay.gameObject.SetActive(false);
        if (blurOverlay != null) blurOverlay.gameObject.SetActive(false);
        EnablePlayerMovement(true);
    }

    private IEnumerator PlayOpeningSequence()
    {
        EnablePlayerMovement(false);

        // ===== Day 1: 2026.3.17 8:00 =====
        yield return StartCoroutine(ShowBlackout("2026.3.17 8:00"));
        yield return StartCoroutine(ShowSubtitle("周二，上班。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // ===== Day 2: 2026.3.18 8:00 =====
        yield return StartCoroutine(ShowBlackout("2026.3.18 8:00"));
        yield return StartCoroutine(ShowSubtitle("周三，去上班。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // ===== Day 3: 2026.3.19 8:00 =====
        yield return StartCoroutine(ShowBlackout("2026.3.19 8:00"));
        yield return StartCoroutine(ShowSubtitle("去上班。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // ===== Day 4: 2026.3.20 8:00 =====
        yield return StartCoroutine(ShowBlackout("2026.3.20 8:00"));
        yield return StartCoroutine(ShowSubtitle("上班。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // ===== Day 5: 2026.3.20 20:00 =====
        yield return StartCoroutine(ShowBlackout("2026.3.20 20:00"));
        yield return StartCoroutine(ShowSubtitle("今天干了什么……？已经完全忘了。也无所谓。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("回家吧。回家。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // 传送到家，面对墙上地铁线路图
        yield return StartCoroutine(FadeOut());
        // 设置玩家位置面对地铁线路图
        if (playerManager != null && subwayMapObject != null)
        {
            Vector3 lookDir = subwayMapObject.transform.position - playerManager.transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                playerManager.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
        yield return StartCoroutine(FadeIn());

        yield return StartCoroutine(ShowSubtitle("你感到极度疲惫。今天的睡觉时间大概是要提前了。"));
        yield return new WaitForSeconds(subtitleDisplayTime);

        // 引导往卧室走
        yield return StartCoroutine(ShowSubtitle("先去卧室吧。"));
        yield return StartCoroutine(FadeSubtitleOut());

        EnablePlayerMovement(true);

        // 启动眩晕监控（偏离引导时触发）
        StartCoroutine(MonitorDizziness());

        // 等待玩家走到卧室（触发区域检测，由外部Trigger调用 OnReachedBedroom）
    }

    /// <summary>
    /// 玩家到达卧室时调用（由卧室Trigger调用）
    /// </summary>
    public void OnReachedBedroom()
    {
        StopCoroutine(nameof(MonitorDizziness));
        StartCoroutine(BedroomSequence());
    }

    private IEnumerator BedroomSequence()
    {
        EnablePlayerMovement(false);

        // 黑屏
        yield return StartCoroutine(FadeOut());

        // 虚化特效（没睁眼的感觉）
        if (blurOverlay != null)
        {
            blurOverlay.gameObject.SetActive(true);
            Color c = blurOverlay.color;
            c.a = 0.8f;
            blurOverlay.color = c;
        }

        yield return new WaitForSeconds(1f);

        // "你在晨光中醒来"
        yield return StartCoroutine(ShowSubtitle("2026.3.21 8:00"));
        yield return new WaitForSeconds(subtitleDisplayTime);

        yield return StartCoroutine(ShowSubtitle("你在晨光中醒来。足足十二小时的悠长睡眠自大学时代之后就不曾有了。这种新奇的体验唤起了 一些掩埋许久的思绪。"));
        yield return new WaitForSeconds(subtitleDisplayTime * 1.5f);

        yield return StartCoroutine(ShowSubtitle("先起床比较好。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // 关虚化特效
        if (blurOverlay != null)
        {
            float elapsed = 0f;
            Color c = blurOverlay.color;
            while (elapsed < blurFadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0.8f, 0f, elapsed / blurFadeDuration);
                blurOverlay.color = c;
                yield return null;
            }
            blurOverlay.gameObject.SetActive(false);
        }

        // 出卧室反思文本
        yield return StartCoroutine(ShowSubtitle("你开始反思。你想到自己两点一线的工作。一份并不讨厌，也谈不上喜欢的工作。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("它的确带来了稳定的生活和不错的收入，让今天的你有余裕思考这些东西。当年的你为工作付出的努力没有白费。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("你想要笑一下，嘴角轻轻扬起——在形成弧度之前，又默然落下去。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("真的是这样吗？那为什么还会不幸福？你问曾经的自己。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("那时候的心境早就忘了。你在等一个永远听不到的回复。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        EnablePlayerMovement(true);

        // 出现三个交互点
        if (interactPromptRoot != null)
        {
            interactPromptRoot.SetActive(true);
        }

        yield return StartCoroutine(ShowSubtitle("你的心和这些东西一起，蒙上了一层灰尘。唯独今天，你有点难以忍受这样的生活了。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(ShowSubtitle("收拾一下，出去看看。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // 等待玩家出门（由门的Trigger调用 OnPlayerLeftHome）
    }

    /// <summary>
    /// 玩家离开家时调用（由门Trigger调用）
    /// </summary>
    public void OnPlayerLeftHome()
    {
        StartCoroutine(LeaveHomeSequence());
    }

    private IEnumerator LeaveHomeSequence()
    {
        EnablePlayerMovement(false);
        yield return StartCoroutine(FadeOut());

        // 引导到青庄（Residential场景）
        yield return StartCoroutine(ShowSubtitle("你走出家门。"));
        yield return new WaitForSeconds(subtitleDisplayTime);
        yield return StartCoroutine(FadeSubtitleOut());

        // 加载小区场景
        SceneManager.LoadScene("Residential");
    }

    // ===== 视觉特效方法 =====

    private IEnumerator ShowBlackout(string dateText)
    {
        yield return StartCoroutine(FadeOut());
        if (subtitleText != null)
        {
            subtitleText.text = dateText;
            subtitleText.alpha = 1f;
            subtitleText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(subtitleDisplayTime);
    }

    private IEnumerator ShowSubtitle(string text)
    {
        if (subtitleText == null) yield break;

        subtitleText.gameObject.SetActive(true);
        subtitleText.text = "";

        // 打字机效果
        foreach (char c in text)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    private IEnumerator FadeSubtitleOut()
    {
        if (subtitleText == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            subtitleText.alpha = Mathf.Lerp(1f, 0f, elapsed / (fadeDuration * 0.5f));
            yield return null;
        }
        subtitleText.gameObject.SetActive(false);
        subtitleText.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (blackoutImage == null) yield break;

        blackoutImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = blackoutImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            blackoutImage.color = c;
            yield return null;
        }
        c.a = 1f;
        blackoutImage.color = c;
    }

    private IEnumerator FadeIn()
    {
        if (blackoutImage == null) yield break;

        float elapsed = 0f;
        Color c = blackoutImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackoutImage.color = c;
            yield return null;
        }
        blackoutImage.gameObject.SetActive(false);
        c.a = 0f;
        blackoutImage.color = c;
    }

    private IEnumerator MonitorDizziness()
    {
        // 偏离引导方向时加眩晕特效
        if (vignetteOverlay == null || playerManager == null) yield break;

        Vector3 bedroomDirection = Vector3.forward; // 需要根据实际卧室方向设置
        while (true)
        {
            Vector3 playerForward = playerManager.transform.forward;
            float dot = Vector3.Dot(playerForward.normalized, bedroomDirection.normalized);

            // 偏离越大，眩晕越强
            float vignetteAlpha = Mathf.Lerp(0f, maxVignetteAlpha, Mathf.Clamp01(1f - dot));
            Color c = vignetteOverlay.color;
            c.a = vignetteAlpha;
            vignetteOverlay.color = c;

            vignetteOverlay.gameObject.SetActive(vignetteAlpha > 0.05f);
            yield return null;
        }
    }

    private void EnablePlayerMovement(bool enabled)
    {
        if (playerManager != null)
        {
            playerManager.enabled = enabled;
        }
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}
