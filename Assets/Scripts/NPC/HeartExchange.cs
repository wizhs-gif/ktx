using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 交心机制 - 核心行为：把自己的心掏出来交给对方
/// 挂载在可触发交心的NPC上（如公园林悦、夕台季阳飞）
/// </summary>
public class HeartExchange : MonoBehaviour
{
    [Header("交心条件")]
    [SerializeField] private string requiredItemId;      // 需要交出的物品ID
    [SerializeField] private string rewardItemId;         // 交心后获得的物品ID（可选）
    [SerializeField] private int dayRequired = -1;        // 需要的天数（-1表示不限）
    [SerializeField] private string prerequisiteFlag;     // 前置标记（可选）

    [Header("动画配置")]
    [SerializeField] private float animationDuration = 3f;
    [SerializeField] private float heartRiseHeight = 1.5f;
    [SerializeField] private GameObject heartEffectPrefab; // 心脏特效预制体
    [SerializeField] private Transform heartSpawnPoint;    // 心脏生成位置

    [Header("屏幕特效")]
    [SerializeField] private float screenShakeIntensity = 0.3f;
    [SerializeField] private float screenShakeDuration = 2f;
    [SerializeField] private float colorShiftDuration = 3f;
    [SerializeField] private float vignetteIntensity = 0.6f;

    [Header("音效")]
    [SerializeField] private AudioClip heartbeatSound;
    [SerializeField] private AudioClip tearSound;

    private bool hasExchanged = false;
    private bool playerInRange = false;
    private KeyCode interactKey = KeyCode.E;

    private void Update()
    {
        if (!playerInRange || hasExchanged) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryStartExchange();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 检查条件
        if (!CanExchange()) return;

        playerInRange = true;
        // 显示交互提示（"按E交心"）
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    private bool CanExchange()
    {
        if (hasExchanged) return false;

        // 检查天数
        if (dayRequired > 0 && GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.CurrentDay < dayRequired) return false;
        }

        // 检查前置标记
        if (!string.IsNullOrEmpty(prerequisiteFlag) && GameDataManager.Instance != null)
        {
            if (!GameDataManager.Instance.GetFlag(prerequisiteFlag)) return false;
        }

        // 检查是否拥有需要交出的物品
        if (!string.IsNullOrEmpty(requiredItemId) && GameDataManager.Instance != null)
        {
            if (!GameDataManager.Instance.HasItem(requiredItemId)) return false;
        }

        return true;
    }

    private void TryStartExchange()
    {
        if (!CanExchange()) return;

        hasExchanged = true;
        StartCoroutine(PlayExchangeSequence());
    }

    private IEnumerator PlayExchangeSequence()
    {
        // 禁用玩家移动
        PlayerManager player = FindObjectOfType<PlayerManager>();
        if (player != null) player.enabled = false;

        // 播放心跳音效
        if (heartbeatSound != null)
        {
            AudioSource.PlayClipAtPoint(heartbeatSound, transform.position);
        }

        // 屏幕震动开始
        StartCoroutine(ScreenShake());

        // 色调偏移（画面变红/暗）
        StartCoroutine(ColorShift());

        // 暗角效果加重
        StartCoroutine(VignetteEffect());

        yield return new WaitForSeconds(0.5f);

        // 生成心脏特效
        if (heartEffectPrefab != null)
        {
            Vector3 spawnPos = heartSpawnPoint != null ?
                heartSpawnPoint.position :
                transform.position + Vector3.up * 1.2f;

            GameObject heart = Instantiate(heartEffectPrefab, spawnPos, Quaternion.identity);

            // 心脏上升动画
            StartCoroutine(HeartRiseAnimation(heart));
        }

        // 撕裂音效
        if (tearSound != null)
        {
            AudioSource.PlayClipAtPoint(tearSound, transform.position);
        }

        yield return new WaitForSeconds(animationDuration);

        // 移除需要交出的物品
        if (!string.IsNullOrEmpty(requiredItemId) && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.RemoveItem(requiredItemId);
        }

        // 给予奖励物品
        if (!string.IsNullOrEmpty(rewardItemId) && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.AddItem(rewardItemId);
        }

        // 精神值升高一级
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.AddSanity(1);
        }

        // 设置标记
        string flagName = $"heart_exchange_{gameObject.name}";
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetFlag(flagName, true);
        }

        // 恢复玩家移动
        if (player != null) player.enabled = true;

        Debug.Log("[HeartExchange] 交心完成");
    }

    private IEnumerator HeartRiseAnimation(GameObject heart)
    {
        Vector3 startPos = heart.transform.position;
        Vector3 endPos = startPos + Vector3.up * heartRiseHeight;
        float elapsed = 0f;

        while (elapsed < animationDuration * 0.8f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (animationDuration * 0.8f);

            // 缓慢上升 + 轻微旋转
            heart.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            heart.transform.Rotate(Vector3.up, 30f * Time.deltaTime);

            // 后半段开始淡出
            if (t > 0.6f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                var renderers = heart.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    var c = r.material.color;
                    c.a = alpha;
                    r.material.color = c;
                }
            }

            yield return null;
        }

        Destroy(heart);
    }

    private IEnumerator ScreenShake()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < screenShakeDuration)
        {
            elapsed += Time.deltaTime;
            float intensity = screenShakeIntensity * (1f - elapsed / screenShakeDuration);

            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            cam.transform.localPosition = originalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    private IEnumerator ColorShift()
    {
        // 使用全局Volume或后处理
        // 如果没有Post Processing Stack，可以用全屏Image替代
        Image colorOverlay = null;

        // 尝试查找场景中的颜色覆盖层
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            var overlayObj = new GameObject("ColorShiftOverlay");
            overlayObj.transform.SetParent(canvas.transform, false);
            colorOverlay = overlayObj.AddComponent<Image>();
            colorOverlay.color = new Color(0.5f, 0f, 0f, 0f);
            colorOverlay.raycastTarget = false;

            // 设置为全屏
            var rt = overlayObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        if (colorOverlay == null) yield break;

        float elapsed = 0f;
        float halfDuration = colorShiftDuration * 0.3f;

        // 红色渐入
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.3f, elapsed / halfDuration);
            colorOverlay.color = new Color(0.5f, 0f, 0f, alpha);
            yield return null;
        }

        // 保持
        yield return new WaitForSeconds(colorShiftDuration * 0.4f);

        // 红色渐出
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.3f, 0f, elapsed / halfDuration);
            colorOverlay.color = new Color(0.5f, 0f, 0f, alpha);
            yield return null;
        }

        Destroy(colorOverlay.gameObject);
    }

    private IEnumerator VignetteEffect()
    {
        // 暗角效果 - 用全屏四个角的黑色Image模拟
        Image vignetteOverlay = null;

        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            var vignetteObj = new GameObject("VignetteOverlay");
            vignetteObj.transform.SetParent(canvas.transform, false);
            vignetteOverlay = vignetteObj.AddComponent<Image>();

            // 使用径向渐变图片作为暗角，如果没有就用半透明黑色
            vignetteOverlay.color = new Color(0f, 0f, 0f, 0f);
            vignetteOverlay.raycastTarget = false;

            var rt = vignetteObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        if (vignetteOverlay == null) yield break;

        float elapsed = 0f;
        float fadeInTime = 1f;
        float holdTime = colorShiftDuration - 2f;
        float fadeOutTime = 1f;

        // 暗角渐入
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, vignetteIntensity, elapsed / fadeInTime);
            vignetteOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // 保持
        yield return new WaitForSeconds(holdTime);

        // 暗角渐出
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(vignetteIntensity, 0f, elapsed / fadeOutTime);
            vignetteOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        Destroy(vignetteOverlay.gameObject);
    }

    /// <summary>
    /// 检查是否已完成交心
    /// </summary>
    public bool HasCompleted()
    {
        return hasExchanged;
    }
}
