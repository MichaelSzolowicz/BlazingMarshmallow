using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitions : MonoBehaviour
{
    public Vector3 spawnPoint = Vector3.zero;

    public bool isLoading = false;

    private string loadLevel = "";

    public float playTime = 0;

    public Dictionary<string, LeaderboardSave> leaderboards;
    private bool leaderboardsLoaded = false;

    public static LevelTransitions instance { get; private set; }

    public LevelTransitions getInstance()
    {
        return instance;
    }

    private void Start()
    {
        //SceneManager.sceneLoaded += Loaded;

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this);

        LoadLeaderboards();
    }


    public void LoadLeaderboards()
    {
        leaderboards = new Dictionary<string, LeaderboardSave>();

        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        foreach(EditorBuildSettingsScene scene in buildScenes)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(scene.path);

            LeaderboardSave save = Serializer.LoadLeaderboard(name);
            if(save != null)
            {
                leaderboards[name] = save;
            }
        }

        leaderboardsLoaded = true;
    }

    private void Awake()
    {
        if (instance != null && instance != this && playTime == 0)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (!leaderboardsLoaded)
        {
            LoadLeaderboards();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Load(string level)
    {
        spawnPoint = Vector3.zero;
        loadLevel = level;
        playTime = 0;

        if (!isLoading)
        {
            StartLoad();
            StartCoroutine(LoadAsyncScene());
        }
    }

    public void ReloadCurrent()
    {
        loadLevel = SceneManager.GetActiveScene().name;

        if (!isLoading) {
            StartLoad();
            StartCoroutine(LoadAsyncScene());
        }
    }
    
    public void ReloadCurrent(bool resetCheckpoints)
    {
        loadLevel = SceneManager.GetActiveScene().name;

        if (resetCheckpoints)
        {
            spawnPoint = Vector3.zero;
            playTime = 0;
        }

        if (!isLoading) {
            StartLoad();
            StartCoroutine(LoadAsyncScene());
        }

    }

    private IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadLevel);

        if(asyncLoad != null)
        {
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                print("loading...");
                yield return null;
            }
        }

        Loaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void Respawn()
    {
        Controller player = FindObjectOfType<Controller>();
        if (player != null && spawnPoint != Vector3.zero)
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
        loadLevel = SceneManager.GetActiveScene().name;
        Invoke("Respawn", .0f);
    }
}