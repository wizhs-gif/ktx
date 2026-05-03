using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景控制器 - 管理场景切换和场景状态
/// </summary>
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("场景配置")]
    [SerializeField] private string homeSceneName = "Home";
    [SerializeField] private string subwaySceneName = "SubWay";

    [Header("UI")]
    [SerializeField] private MapNameDisplay mapNameDisplay;

    private string currentSceneName;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        // 更新地图名称显示
        if (mapNameDisplay != null)
        {
            mapNameDisplay.ShowMapName(GetMapDisplayName(currentSceneName));
        }

        // 根据场景执行初始化逻辑
        InitializeScene(currentSceneName);
    }

    private void InitializeScene(string sceneName)
    {
        // 根据场景名称执行不同的初始化逻辑
        switch (sceneName)
        {
            case "Hospital":
                // 医院场景初始化
                Debug.Log("[SceneController] 初始化医院场景");
                break;

            case "OfficeBuilding":
                // 办公楼场景初始化
                Debug.Log("[SceneController] 初始化办公楼场景");
                break;

            case "Square":
                // 广场场景初始化
                Debug.Log("[SceneController] 初始化广场场景");
                break;

            case "Park":
                // 公园场景初始化
                Debug.Log("[SceneController] 初始化公园场景");
                break;

            case "Home":
                // 家场景初始化
                Debug.Log("[SceneController] 初始化家场景");
                break;
        }
    }

    private string GetMapDisplayName(string sceneName)
    {
        switch (sceneName)
        {
            case "Hospital": return MapNames.HOSPITAL;
            case "OfficeBuilding": return MapNames.OFFICE;
            case "Square": return MapNames.SQUARE;
            case "Park": return MapNames.PARK;
            case "Home": return MapNames.HOME;
            case "SubWay": return MapNames.SUBWAY;
            case "Cemetery": return MapNames.CEMETERY;
            case "SciencePark": return MapNames.SCIENCE_PARK;
            case "Commercial": return MapNames.COMMERCIAL;
            default: return sceneName;
        }
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    public void LoadScene(string sceneName)
    {
        // 推进时间
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.AdvanceTime();
        }

        // 加载场景
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 传送到家
    /// </summary>
    public void GoHome()
    {
        LoadScene(homeSceneName);
    }

    /// <summary>
    /// 前往地铁
    /// </summary>
    public void GoToSubway()
    {
        LoadScene(subwaySceneName);
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
}
