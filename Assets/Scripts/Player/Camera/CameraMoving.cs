using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public GameObject box;
    public GameObject player;
    public float playerSpeed;
    public float fov;

    public int fovMax;
    public int fovMin;

    
    public float yPosCentered;
    public float yPosCurrent;
    public float yPosMax;
    public float yPosMin;
    private float yBase;
    // Start is called before the first frame update

    void Start()
    {
        SetFov();
        yPosCentered = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        CameraFOV();
        setYCam();
        CurrentCameraY();
    }
    private void FixedUpdate()
    {
        CameraVerticle();
        CameraRamp();
    }

    private void setYCam()
    {
        yBase = transform.position.y;
    }

    private void CurrentCameraY()
    {
        yBase = yPosCurrent;
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
    

    public void CameraRamp()
    {
        playerSpeed = player.GetComponent<Controller>().currentSpeed + 10;
        if (playerSpeed <= fov)
        {
            if (fov >= fovMin)
            {
                fov = (fov - 10 * Time.deltaTime);
            }           
        }
        if (playerSpeed >= fov)
        {
            if (fov <= fovMax)
            {
                fov = (fov + 10 * Time.deltaTime);
            }
        }
    }

    public void CameraVerticle()
    {
        
        if (yPosCurrent >= yPosCentered)
        {
            if (yPosCurrent >= yPosMin)
            {
                yPosCurrent = ((yPosCurrent -1) * Time.deltaTime);
            }
        }
        if (yPosCurrent <= yPosCentered)
        {
            if (yPosCurrent <= yPosMax)
            {
                yPosCurrent = ((yPosCurrent + 1) * Time.deltaTime);
            }
        }
    }
    

}
