using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器 - 管理所有UI面板的显示和隐藏
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI面板")]
    [SerializeField] private GameObject hudPanel;           // HUD面板
    [SerializeField] private GameObject dialoguePanel;      // 对话面板
    [SerializeField] private GameObject itemPanel;          // 物品面板
    [SerializeField] private GameObject timeSelectPanel;    // 时间选择面板
    [SerializeField] private GameObject dayEndPanel;        // 每日结束面板
    [SerializeField] private GameObject menuPanel;          // 菜单面板

    private Stack<GameObject> panelStack = new Stack<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始化时隐藏所有面板
        HideAllPanels();

        // 显示HUD
        ShowHUD();
    }

    /// <summary>
    /// 场景加载后重新绑定场景内的UI面板引用
    /// 由 SceneController.OnSceneLoaded 自动调用
    /// </summary>
    public void RebindScenePanels()
    {
        HUDManager hud = FindObjectOfType<HUDManager>();
        if (hud != null) hudPanel = hud.gameObject;

        DialogueManager dialogue = FindObjectOfType<DialogueManager>();
        if (dialogue != null) dialoguePanel = dialogue.gameObject;

        TimeSelectionUI timeSelect = FindObjectOfType<TimeSelectionUI>();
        if (timeSelect != null) timeSelectPanel = timeSelect.gameObject;

        DayEndUI dayEnd = FindObjectOfType<DayEndUI>();
        if (dayEnd != null) dayEndPanel = dayEnd.gameObject;

        // 重新隐藏所有面板，再显示HUD
        HideAllPanels();
        ShowHUD();

        Debug.Log("[UIManager] 场景UI面板已重新绑定");
    }

    // ==================== 面板显示控制 ====================

    /// <summary>
    /// 显示指定面板
    /// </summary>
    public void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(true);
        panelStack.Push(panel);

        Debug.Log($"[UIManager] 显示面板: {panel.name}");
    }

    /// <summary>
    /// 隐藏指定面板
    /// </summary>
    public void HidePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);

        // 从栈中移除
        if (panelStack.Count > 0 && panelStack.Peek() == panel)
        {
            panelStack.Pop();
        }

        Debug.Log($"[UIManager] 隐藏面板: {panel.name}");
    }

    /// <summary>
    /// 隐藏当前最上层面板
    /// </summary>
    public void HideTopPanel()
    {
        if (panelStack.Count > 0)
        {
            GameObject topPanel = panelStack.Pop();
            topPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    public void HideAllPanels()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (itemPanel != null) itemPanel.SetActive(false);
        if (timeSelectPanel != null) timeSelectPanel.SetActive(false);
        if (dayEndPanel != null) dayEndPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(false);

        panelStack.Clear();
    }

    // ==================== 特定面板控制 ====================

    /// <summary>
    /// 显示HUD
    /// </summary>
    public void ShowHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏HUD
    /// </summary>
    public void HideHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示对话面板
    /// </summary>
    public void ShowDialogue()
    {
        ShowPanel(dialoguePanel);
    }

    /// <summary>
    /// 显示物品面板
    /// </summary>
    public void ShowItemPanel()
    {
        ShowPanel(itemPanel);
    }

    /// <summary>
    /// 显示时间选择面板
    /// </summary>
    public void ShowTimeSelect()
    {
        ShowPanel(timeSelectPanel);
    }

    /// <summary>
    /// 显示每日结束面板
    /// </summary>
    public void ShowDayEnd()
    {
        ShowPanel(dayEndPanel);
    }

    /// <summary>
    /// 显示菜单
    /// </summary>
    public void ShowMenu()
    {
        ShowPanel(menuPanel);
    }

    // ==================== 面板状态查询 ====================

    /// <summary>
    /// 检查是否有面板打开
    /// </summary>
    public bool IsAnyPanelOpen()
    {
        return panelStack.Count > 0;
    }

    /// <summary>
    /// 检查指定面板是否打开
    /// </summary>
    public bool IsPanelOpen(GameObject panel)
    {
        return panel != null && panel.activeSelf;
    }
}
