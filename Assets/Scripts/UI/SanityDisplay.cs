using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 精神值显示组件 - 可挂载在任何UI元素上
/// </summary>
public class SanityDisplay : MonoBehaviour
{
    [Header("显示模式")]
    [SerializeField] private DisplayMode displayMode = DisplayMode.Icons;

    [Header("图标模式")]
    [SerializeField] private Image[] sanityIcons;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.3f);

    [Header("进度条模式")]
    [SerializeField] private Slider sanitySlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient sanityGradient;

    [Header("文本模式")]
    [SerializeField] private TextMeshProUGUI sanityText;
    [SerializeField] private string textFormat = "{0}/5";

    [Header("颜色配置")]
    [SerializeField] private Color[] sanityColors = new Color[]
    {
        Color.red,                      // 0档
        new Color(1f, 0.3f, 0f),       // 1档
        new Color(1f, 0.6f, 0f),       // 2档
        Color.yellow,                   // 3档
        Color.green,                    // 4档
        Color.cyan                      // 5档
    };

    public enum DisplayMode
    {
        Icons,
        Slider,
        Text
    }

    private void OnEnable()
    {
        GameEvents.OnSanityChanged += OnSanityChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnSanityChanged -= OnSanityChanged;
    }

    private void Start()
    {
        // 初始化
        if (GameDataManager.Instance != null)
        {
            UpdateDisplay(GameDataManager.Instance.Sanity);
        }
    }

    private void OnSanityChanged(int oldVal, int newVal)
    {
        UpdateDisplay(newVal);
    }

    private void UpdateDisplay(int sanity)
    {
        switch (displayMode)
        {
            case DisplayMode.Icons:
                UpdateIcons(sanity);
                break;
            case DisplayMode.Slider:
                UpdateSlider(sanity);
                break;
            case DisplayMode.Text:
                UpdateText(sanity);
                break;
        }
    }

    private void UpdateIcons(int sanity)
    {
        if (sanityIcons == null) return;

        for (int i = 0; i < sanityIcons.Length; i++)
        {
            if (sanityIcons[i] == null) continue;

            bool isActive = i < sanity;

            if (activeSprite != null && inactiveSprite != null)
            {
                sanityIcons[i].sprite = isActive ? activeSprite : inactiveSprite;
            }

            sanityIcons[i].color = isActive ? activeColor : inactiveColor;
        }
    }

    private void UpdateSlider(int sanity)
    {
        if (sanitySlider != null)
        {
            sanitySlider.value = sanity;
        }

        if (fillImage != null)
        {
            fillImage.color = GetSanityColor(sanity);
        }
    }

    private void UpdateText(int sanity)
    {
        if (sanityText != null)
        {
            sanityText.text = string.Format(textFormat, sanity);
            sanityText.color = GetSanityColor(sanity);
        }
    }

    private Color GetSanityColor(int sanity)
    {
        int index = Mathf.Clamp(sanity, 0, sanityColors.Length - 1);
        return sanityColors[index];
    }
}
