using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectableImage : MonoBehaviour
{
    [SerializeField] private Outline outline;  
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    public string sceneName;

    private Coroutine blinkCoroutine;
    public bool IsSelected { get; private set; }

    void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        outline.enabled = false;
        

    }

    public void OnClick()
    {
        
        ImageSelectionManager.Instance.SelectImage(this);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (selected)
        {
            outline.enabled = true;
            if (blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(BlinkOutline());
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            outline.enabled = false;
        }
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
}
