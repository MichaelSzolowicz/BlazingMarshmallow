using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public GameObject box;
    public GameObject player;
    public float playerSpeed;
    public float fov;
    // Start is called before the first frame update

    void Start()
    {
        SetFov();
    }

    // Update is called once per frame
    void Update()
    {

        SpeedUpdate();
        CameraFOV();
    }

    public void SetFov()
    {
        //get the cameras field of view
        fov = GetComponent<Camera>().GetComponent<Camera>().fieldOfView;
    }
    public void CameraFOV()
    {
        GetComponent<Camera>().GetComponent<Camera>().fieldOfView = fov;
    }
    public void SpeedUpdate()
    {
        playerSpeed = player.GetComponent<Controller>().currentSpeed;

        fov = (playerSpeed / 2) + 60;
    }
}
