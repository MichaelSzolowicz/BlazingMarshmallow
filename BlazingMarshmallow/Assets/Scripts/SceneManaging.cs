using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    public GameObject Lights;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.M))
        {
            SceneManager.LoadScene("Manu");
            Debug.Log("Menu");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //close the game
            Application.Quit();
            
        }
    }

    //generate a code that will let me have buttons that will load the chosen scene
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //genertae a code that will let me have an object that will not be destroyed on loading a new scene
    public void DontDestroyOnLoad()
    {
        DontDestroyOnLoad(Lights);
    }

}
