using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenButtons : MonoBehaviour
{
    public void NextLevel(string level)
    {
        LevelTransitions levels = LevelTransitions.instance;
        if (levels != null)
        {
            levels.Load(level);
        }
    }

    public void Retry()
    {
        LevelTransitions levels = LevelTransitions.instance; 
        if(levels != null)
        {
            levels.ReloadCurrent(true);
        }
    }

    public void Quit()
    {
        print("Quit");
        Application.Quit();
    }
}
