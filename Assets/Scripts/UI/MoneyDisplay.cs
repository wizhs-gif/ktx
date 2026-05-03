using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 金钱显示组件 - 显示金钱变化动画
/// </summary>
public class MoneyDisplay : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image moneyIcon;

    [Header("格式")]
    [SerializeField] private string moneyFormat = "¥{0}";
    [SerializeField] private string moneyFormatWithSign = "¥{0}{1}";

    [Header("动画")]
    [SerializeField] private bool enableAnimation = true;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("颜色")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color increaseColor = Color.green;
    [SerializeField] private Color decreaseColor = Color.red;
    [SerializeField] private float colorFlashDuration = 0.3f;

    private int displayValue;
    private Coroutine animateCoroutine;
    private Coroutine flashCoroutine;

    private void OnEnable()
    {
        GameEvents.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnMoneyChanged -= OnMoneyChanged;
    }

    private void Start()
    {
        // 初始化显示
        if (GameDataManager.Instance != null)
        {
            displayValue = GameDataManager.Instance.Money;
            UpdateText(displayValue);
        }
    }

    private void OnMoneyChanged(int oldVal, int newVal)
    {
        if (enableAnimation)
        {
            // 停止之前的动画
            if (animateCoroutine != null)
            {
                StopCoroutine(animateCoroutine);
            }
            animateCoroutine = StartCoroutine(AnimateMoneyChange(oldVal, newVal));
        }
        else
        {
            displayValue = newVal;
            UpdateText(newVal);
        }

        // 颜色闪烁
        if (newVal > oldVal)
        {
            FlashColor(increaseColor);
        }
        else if (newVal < oldVal)
        {
            FlashColor(decreaseColor);
        }
    }

    private IEnumerator AnimateMoneyChange(int from, int to)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = animationCurve.Evaluate(elapsed / animationDuration);
            displayValue = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            UpdateText(displayValue);
            yield return null;
        }

        displayValue = to;
        UpdateText(to);
        animateCoroutine = null;
    }

    private void UpdateText(int value)
    {
        if (moneyText != null)
        {
            moneyText.text = string.Format(moneyFormat, value);
        }
    }

    private void FlashColor(Color flashColor)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashColorCoroutine(flashColor));
    }

    private IEnumerator FlashColorCoroutine(Color flashColor)
    {
        if (moneyText != null)
        {
            moneyText.color = flashColor;
            yield return new WaitForSeconds(colorFlashDuration);

            // 渐变回正常颜色
            float elapsed = 0f;
            while (elapsed < colorFlashDuration)
            {
                elapsed += Time.deltaTime;
                moneyText.color = Color.Lerp(flashColor, normalColor, elapsed / colorFlashDuration);
                yield return null;
            }

            moneyText.color = normalColor;
        }

        flashCoroutine = null;
    }

    /// <summary>
    /// 立即更新显示（不带动画）
    /// </summary>
    public void ForceUpdate()
    {
        if (GameDataManager.Instance != null)
        {
            displayValue = GameDataManager.Instance.Money;
            UpdateText(displayValue);
            moneyText.color = normalColor;
        }
    }
}
