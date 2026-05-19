using UnityEngine;

/// <summary>
/// 场景启动器 - 挂在每个场景的根物体上，负责初始化场景并驱动游戏流程
/// Home 场景：开场结束后启动第一天
/// 外出场景：加载后自动打开地铁选站
/// </summary>
public class SceneBootstrapper : MonoBehaviour
{
    public enum SceneType
    {
        Home,       // 家（开场 + 每日开始）
        Subway,     // 地铁（选站）
        Outdoor     // 外出场景（直接进入）
    }

    [Header("场景配置")]
    [SerializeField] private SceneType sceneType = SceneType.Outdoor;

    [Header("Home 场景引用")]
    [SerializeField] private OpeningSequence openingSequence;

    private void Start()
    {
        switch (sceneType)
        {
            case SceneType.Home:
                InitHomeScene();
                break;
            case SceneType.Subway:
                InitSubwayScene();
                break;
            case SceneType.Outdoor:
                InitOutdoorScene();
                break;
        }
    }

    private void InitHomeScene()
    {
        // 如果开场没播完，让 OpeningSequence 自己处理
        if (GameDataManager.Instance != null &&
            !GameDataManager.Instance.GetFlag("opening_completed"))
        {
            // OpeningSequence 会在 Start 里自动播放
            return;
        }

        // 开场已完成，直接开始当天流程
        OnOpeningComplete();
    }

    private void InitSubwayScene()
    {
        // 地铁场景：打开选站 UI
        SubwayMapUI subwayMap = FindObjectOfType<SubwayMapUI>();
        if (subwayMap != null)
        {
            subwayMap.OpenSubway(OnStationSelected);
        }
    }

    private void InitOutdoorScene()
    {
        // 外出场景：直接可用，HUD 自动通过事件更新
        Debug.Log($"[SceneBootstrapper] 进入场景: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
    }

    /// <summary>
    /// 开场剧情结束后调用（由 OpeningSequence 触发）
    /// </summary>
    public void OnOpeningComplete()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.StartDay();
        }
    }

    /// <summary>
    /// 地铁选站完成后调用
    /// </summary>
    private void OnStationSelected(string sceneName)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
