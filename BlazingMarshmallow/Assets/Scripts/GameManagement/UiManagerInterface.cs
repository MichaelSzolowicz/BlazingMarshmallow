using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerInterface : MonoBehaviour
{
    protected LevelTransitions levels;

    void Start()
    {
        levels = LevelTransitions.instance;
    }

    public void Quit()
    {
        levels.Quit();
    }

    public void Load(string level)
    {
        levels.Load(level);
    }

    public void ReloadCurrent()
    {
        levels.ReloadCurrent();
    }

    public void ReloadCurrent(bool resetCheckpoints)
    {
        levels.ReloadCurrent(resetCheckpoints);

    }

}
