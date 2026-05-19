using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("按钮")]
    public Button StartButton;
    public Button ContinueButton;
    public Button EndButton;

    [Header("退出确认面板")]
    public GameObject EndPanel;
    public Button yes;
    public Button no;

    [Header("游戏启动器")]
    [SerializeField] private GameBootstrap gameBootstrap;

    void Start()
    {
        StartButton.onClick.AddListener(GameStart);
        ContinueButton.onClick.AddListener(ContinueGame);
        EndButton.onClick.AddListener(EndGame);

        EndPanel.SetActive(false);
        yes.onClick.AddListener(SureEnd);
        no.onClick.AddListener(RefuseEnd);

        // 没有存档时禁用继续按钮
        if (SaveSystem.Instance != null && !SaveSystem.Instance.HasSave())
        {
            ContinueButton.interactable = false;
        }
    }

    void GameStart()
    {
        Debug.Log("开始新游戏");

        if (gameBootstrap != null)
        {
            gameBootstrap.StartNewGame();
        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }

    void ContinueGame()
    {
        Debug.Log("继续游戏");

        if (gameBootstrap != null)
        {
            gameBootstrap.ContinueGame();
        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }

    void EndGame()
    {
        EndPanel.SetActive(true);
    }

    void SureEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void RefuseEnd()
    {
        EndPanel.SetActive(false);
    }
}
