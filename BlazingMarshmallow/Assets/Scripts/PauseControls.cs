using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControls : MonoBehaviour
{
    private PlayerInput playerController;

    protected bool isPaused = false;

    [SerializeField] protected GameObject pauseScreen;

    private void Awake()
    {
        // Initialize our input actions.
        playerController = new PlayerInput();
        playerController.Enable();

        playerController.Controls.Pause.started += PauseCallback;
    }

    private void PauseCallback(InputAction.CallbackContext context)
    {
        Pause();
    }

    public void Pause()
    {
        if (Time.timeScale != 0)
        {
            print("PC pause");
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            isPaused = true;
        }
        else if (isPaused)
        {
            print("PC unpause");
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            isPaused = false;
        }
    }

    private void OnDestroy()
    {
        playerController.Controls.Pause.started -= PauseCallback;
    }
}
