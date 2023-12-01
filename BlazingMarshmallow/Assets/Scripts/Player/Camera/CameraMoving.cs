using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Fov Speed (default 10): How agressviely the FOV tries te xpand. High value can cause it to get stuck at max FOV.")]
    public float fovSpeed = 10;
    [Header("Sensitivy (default 0.6): Mitigates jitter. Should be left around 0.5f - 1.0f")]
    public float sensitivity = .6f;
    [Header("Interp Speed (default 20): how fast to interpolate to the target FOV. Very high values can cause stutter when zooming in / out.")]
    public float interpSpeed = 20;

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

        CameraVerticle();
        CameraRamp();
    }
    private void FixedUpdate()
    {

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
        float currFov = GetComponent<Camera>().GetComponent<Camera>().fieldOfView;

        if(Mathf.Abs(fov - currFov) < sensitivity)
        {
            return;
        }

        currFov = Mathf.LerpUnclamped(currFov, fov, Time.deltaTime * interpSpeed);
        GetComponent<Camera>().GetComponent<Camera>().fieldOfView = currFov;
    }

    public void CameraRamp()
    {
        playerSpeed = player.GetComponent<Controller>().currentSpeed + fovSpeed;
        if (playerSpeed <= fov)
        {
            if (fov >= fovMin)
            {
                fov = (fov - fovSpeed * Time.deltaTime);
            }           
        }
        if (playerSpeed >= fov)
        {
            if (fov <= fovMax)
            {
                fov = (fov + fovSpeed * Time.deltaTime);
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
