using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitions : MonoBehaviour
{
    public Vector3 spawnPoint = Vector3.zero;

    public bool isLoading = false;

    private void Start()
    {
        SceneManager.sceneLoaded += Loaded;

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this);

        LevelTransitions other = FindObjectOfType<LevelTransitions>();  
        if(other != this)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
    }

    public void ReloadCurrent()
    {

        if (!isLoading) {
            StartLoad();
            StartCoroutine(LoadAsyncScene());
        }

    }
    
    public void ReloadCurrent(bool resetCheckpoints)
    {
        if(resetCheckpoints)
        {
            spawnPoint = Vector3.zero;
        }

        if (!isLoading) {
            StartLoad();
            StartCoroutine(LoadAsyncScene());
        }

    }

    private IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        if(asyncLoad != null)
        {
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                print("loading...");
                yield return null;
            }
        }

    }

    private void Respawn()
    {
        Controller player = FindObjectOfType<Controller>();
        if (player != null)
        {
            print(this.gameObject.name + " reset " + player.gameObject.name + " to " + spawnPoint);
            player.gameObject.transform.position = spawnPoint;
        }
    }

    private void StartLoad()
    {
        isLoading = true;  
    }

    private void Loaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1.0f;
        isLoading = false;
        Invoke("Respawn", .0f);
    }
}