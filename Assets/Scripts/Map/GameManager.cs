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
    void Start()
    {
        StartButton.onClick.AddListener(GameStart);
        NameListButton.onClick.AddListener(ShowNameList);
        EndButton.onClick.AddListener(EndGame);
        EndPanel.SetActive(false);
        yes.onClick.AddListener(SureEnd);
        no.onClick.AddListener(RefuseEnd);


    }

    // Update is called once per frame
    void GameStart()
    {
        Debug.Log("游戏开始");
        SceneManager.LoadScene("Map");
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
