using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // 添加这个命名空间
using System.Collections;
using UnityEngine.SceneManagement;

public class SelectableImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  // 添加接口
{
    [SerializeField] private Outline outline;  
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    public string sceneName;

    private Coroutine blinkCoroutine;
    public bool IsSelected { get; private set; }
    private bool isHovering = false;  // 添加悬停标志

    void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    // 鼠标进入时开始闪动
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        // 只在未选中状态下闪动
        if (!IsSelected)
        {
            StartBlinking();
        }
    }

    // 鼠标离开时停止闪动
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        // 停止闪动并隐藏轮廓
        if (!IsSelected)
        {
            StopBlinking();
            outline.enabled = false;
        }
    }

    public void OnClick()
    {
        ImageSelectionManager.Instance.SelectImage(this);
        SceneManager.LoadScene(sceneName);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (selected)
        {
            // 选中时停止悬停闪动，启用轮廓
            if (isHovering)
            {
                StopBlinking();
            }
            outline.enabled = true;
            outline.effectColor = highlightColor;  // 设置固定颜色
            outline.effectColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 1);
        }
        else
        {
            // 取消选中时，如果鼠标还在悬停，则恢复闪动
            outline.enabled = false;
            if (isHovering)
            {
                StartBlinking();
            }
        }
    }

    private void StartBlinking()
    {
        if (blinkCoroutine == null)
        {
            outline.enabled = true;
            blinkCoroutine = StartCoroutine(BlinkOutline());
        }
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        outline.enabled = false;
    }

    private IEnumerator BlinkOutline()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime * blinkSpeed;
            float alpha = (Mathf.Sin(t * Mathf.PI) + 1) / 2;
            Color c = highlightColor;
            c.a = alpha;
            outline.effectColor = c;
            yield return null;
        }
    }

    void Start()
    {
        ImageSelectionManager.Instance.RegisterImage(this);
        Debug.Log("SelectableImage Start");
    }

    // 可选：添加鼠标进入/离开的调试信息
    void OnDisable()
    {
        // 组件禁用时停止协程
        StopBlinking();
    }
}