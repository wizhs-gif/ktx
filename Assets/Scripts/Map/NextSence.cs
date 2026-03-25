using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSence : MonoBehaviour
{
    public string nextSenceName;
    
    public void NextSenceButton()
    {
        Debug.Log("Next Sence Button Clicked");
        SceneManager.LoadScene(nextSenceName);
    }
}
