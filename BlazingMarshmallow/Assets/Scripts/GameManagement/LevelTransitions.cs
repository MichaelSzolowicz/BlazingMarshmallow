using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitions : MonoBehaviour
{
    public Vector3 spawnPoint = Vector3.zero;

    public bool isLoading = false;

    private string loadLevel = "";

    public float playTime = 0;

    public int BigChoco = 0;

    public int ChocoBites = 0;

    public static Dictionary<string, LeaderboardSave> leaderboards { get; private set; }
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

    }


    public void LoadLeaderboards()
    {
        LevelTransitions.leaderboards = new Dictionary<string, LeaderboardSave>();

        //EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        int count = SceneManager.sceneCountInBuildSettings;
        for(int i = 0; i < count; i++)
        {
            string name = Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));

            LeaderboardSave save = Serializer.LoadLeaderboard(name);
            if(save != null)
            {
                LevelTransitions.leaderboards[name] = save;
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
            //LoadLeaderboards();
        }

        LoadLeaderboards();
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
        ChocoBites = 0;
        BigChoco = 0;

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
            ChocoBites = 0;
            BigChoco = 0;
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