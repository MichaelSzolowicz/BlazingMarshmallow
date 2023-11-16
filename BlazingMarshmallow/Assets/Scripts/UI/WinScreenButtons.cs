using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenButtons : MonoBehaviour
{
    public void NextLevel(string level)
    {
        LevelTransitions levels = FindObjectOfType<LevelTransitions>();
        if (levels != null)
        {
            levels.Load(level);
        }
        else
        {
            levels = new LevelTransitions();
            levels.Load(level);
        }
    }

    public void Retry()
    {
        LevelTransitions levels = FindObjectOfType<LevelTransitions>(); 
        if(levels != null)
        {
            levels.ReloadCurrent(true);
        }
        else
        {
            levels = new LevelTransitions();
            levels.ReloadCurrent(true); 
        }
    }

    public void Quit()
    {
        print("Quit");
        Application.Quit();
    }
}
