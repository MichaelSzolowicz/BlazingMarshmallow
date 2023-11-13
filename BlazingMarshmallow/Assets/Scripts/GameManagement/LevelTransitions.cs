using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitions : MonoBehaviour
{
    public Vector3 spawnPoint = Vector3.zero;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        LevelTransitions other = FindObjectOfType<LevelTransitions>();  
        if(other != this)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
    }

    public void ReloadCurrent()
    {
        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

        Respawn();
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
}