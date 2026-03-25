using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene1Manager : MonoBehaviour
{
    // Start is called before the first frame update 
    public Button Btn;
    void Start()
    {
       Btn.onClick.AddListener(StartGame);
        
    }

    // Update is called once per frame
    void StartGame()
    {
        SceneManager.LoadScene("Start");

    }
}
