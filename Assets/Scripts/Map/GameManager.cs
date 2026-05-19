using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Button StartButton;
    public Button NameListButton;
    public Button EndButton;
    public GameObject EndPanel;
    public Button yes;
    public Button no;
    public Button NewGame;

    [Header("游戏启动器")]
    [SerializeField] private GameBootstrap gameBootstrap;

    void Start()
    {
        StartButton.onClick.AddListener(GameStart);
        NameListButton.onClick.AddListener(ShowNameList);
        EndButton.onClick.AddListener(EndGame);
        EndPanel.SetActive(false);
        yes.onClick.AddListener(SureEnd);
        no.onClick.AddListener(RefuseEnd);
    }

    void GameStart()
    {
        Debug.Log("游戏开始");

        if (gameBootstrap != null)
        {
            gameBootstrap.StartNewGame();
        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }

    void ShowNameList()
    {
        SceneManager.LoadScene("NameList");
    }
    void EndGame()
    {
        EndPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    void SureEnd()
    {
        Application.Quit();
#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    void RefuseEnd()
    {
        EndPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
}
