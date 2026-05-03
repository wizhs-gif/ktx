using UnityEngine;
using TMPro;

/// <summary>
/// 地图名称显示组件 - 显示当前所在地图名称
/// </summary>
public class MapNameDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI mapNameText;

    [Header("动画")]
    [SerializeField] private bool enableFadeAnimation = true;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("格式")]
    [SerializeField] private string mapNameFormat = "【{0}】";

    private string currentMapName;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // 初始化时不显示
        if (mapNameText != null)
        {
            mapNameText.alpha = 0f;
        }
    }

    /// <summary>
    /// 显示地图名称
    /// </summary>
    public void ShowMapName(string mapName)
    {
        if (string.IsNullOrEmpty(mapName)) return;

        currentMapName = mapName;

        if (enableFadeAnimation)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(ShowMapNameAnimation());
        }
        else
        {
            if (mapNameText != null)
            {
                mapNameText.text = string.Format(mapNameFormat, mapName);
                mapNameText.alpha = 1f;
            }
        }
    }

    private System.Collections.IEnumerator ShowMapNameAnimation()
    {
        if (mapNameText == null) yield break;

        // 设置文本
        mapNameText.text = string.Format(mapNameFormat, currentMapName);

        // 淡入
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            mapNameText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        mapNameText.alpha = 1f;

        // 显示等待
        yield return new WaitForSeconds(displayDuration);

        // 淡出
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            mapNameText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        mapNameText.alpha = 0f;

        fadeCoroutine = null;
    }

    /// <summary>
    /// 立即隐藏
    /// </summary>
    public void HideImmediate()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (mapNameText != null)
        {
            mapNameText.alpha = 0f;
        }
    }
}

/// <summary>
/// 地图名称常量
/// </summary>
public static class MapNames
{
    public const string HOME = "家";
    public const string SUBWAY = "地铁";
    public const string HOSPITAL = "医院";
    public const string OFFICE = "办公写字楼";
    public const string SQUARE = "广场";
    public const string COMMUNITY = "小区";
    public const string CEMETERY = "万安园";
    public const string PARK = "林翠公园";
    public const string SCIENCE_PARK = "生命科学园";
    public const string COMMERCIAL = "夕台";
    public const string OFFICE_INDOOR = "办公室";
    public const string APARTMENT = "通平苑";
}
