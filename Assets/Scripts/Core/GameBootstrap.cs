using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏启动器 - 负责初始化所有核心系统
/// 必须在游戏启动场景中挂载
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("数据库配置")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private NPCDatabase npcDatabase;
    [SerializeField] private GameConfig gameConfig;

    [Header("系统预制体")]
    [SerializeField] private GameObject gameDataManagerPrefab;
    [SerializeField] private GameObject saveSystemPrefab;
    [SerializeField] private GameObject interactSystemPrefab;

    private static bool isInitialized = false;

    private void Awake()
    {
        if (isInitialized)
        {
            // 已经初始化过，跳过重复初始化
            Destroy(gameObject);
            return;
        }

        InitializeGame();
        isInitialized = true;
    }

    private void InitializeGame()
    {
        Debug.Log("[GameBootstrap] 开始初始化游戏系统...");

        // 1. 初始化数据管理器
        if (GameDataManager.Instance == null)
        {
            if (gameDataManagerPrefab != null)
            {
                Instantiate(gameDataManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("GameDataManager");
                go.AddComponent<GameDataManager>();
            }
        }

        // 2. 初始化存档系统
        if (SaveSystem.Instance == null)
        {
            if (saveSystemPrefab != null)
            {
                Instantiate(saveSystemPrefab);
            }
            else
            {
                GameObject go = new GameObject("SaveSystem");
                go.AddComponent<SaveSystem>();
            }
        }

        // 3. 初始化交互系统
        if (InteractSystem.Instance == null)
        {
            if (interactSystemPrefab != null)
            {
                Instantiate(interactSystemPrefab);
            }
            else
            {
                GameObject go = new GameObject("InteractSystem");
                go.AddComponent<InteractSystem>();
            }
        }

        // 4. 初始化数据库
        if (itemDatabase != null)
        {
            itemDatabase.Initialize();
            Debug.Log("[GameBootstrap] 物品数据库已初始化");
        }

        if (npcDatabase != null)
        {
            npcDatabase.Initialize();
            Debug.Log("[GameBootstrap] NPC数据库已初始化");
        }

        // 5. 注册事件监听
        RegisterEvents();

        Debug.Log("[GameBootstrap] 游戏系统初始化完成");
    }

    private void RegisterEvents()
    {
        // 监听每日结束事件，自动保存
        GameEvents.OnDayEnded += OnDayEnded;
    }

    private void OnDayEnded(int day)
    {
        // 每日结束时自动保存
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.AutoSaveOnDayEnd();
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnDayEnded -= OnDayEnded;
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewGame()
    {
        // 重置所有数据
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetAllData();
        }

        // 删除旧存档
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();
        }

        // 加载第一天场景
        SceneManager.LoadScene("Day1_Start");
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            SaveSystem.Instance.LoadGame();
            // 加载对应天数的场景
            int day = GameDataManager.Instance.CurrentDay;
            SceneManager.LoadScene($"Day{day}_Start");
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] 没有找到存档");
        }
    }
}
