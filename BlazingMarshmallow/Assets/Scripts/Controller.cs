using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

//Kriger Nathan & Michael Szolowicz

public class Controller : MonoBehaviour
{
    private Vector3 spawnPoint;

    [Header("Movement Defaults")]
    public float forwardThrust = 1000f;
    public float horizontalThrust;
    public float runThrustScale = 2f;
    public float maxDefaultSpeed = 50f;
    public float maxRunSpeed = 80f;
    public float minSpeed = 0;
    public float strafeSpeed = 5f;
    public float burnSpeedMultiplier = 1.5f;
    private float maxSpeed;
    private float internalBurnSpeedMultiplier;
    public float boostScale = .7f;
    public float slowScale = .1f;
    public float groundProbeDepth = 2f;
    public float deathPlaneDepth = 10;

    [Header("Jump")]
    public float baseJumpForce = 5f;
    public float burnJumpForce = 8f;
    private float jumpForce = 1f;

    //define a reference to our input actions.
    private PlayerInput playerController;

    private bool grounded = false;

    public float currentSpeed;
    public float playerHeight = 1;

    private void Awake()
    {
        // Initialize our input actions.
        playerController = new PlayerInput();
        playerController.Enable();

        // Add jump callback.
        playerController.Controls.Jump.performed += Jump;

        GetComponent<Rigidbody>().velocity = transform.forward * maxDefaultSpeed;
        maxSpeed = maxDefaultSpeed;
        jumpForce = baseJumpForce;
        internalBurnSpeedMultiplier = 1;
        spawnPoint = transform.position;
        GrappleHook grapple = GetComponent<GrappleHook>();
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();
        if (playerStats != null)
        {
            playerStats.AddInflictBurnCallback(speedBoost);
            playerStats.AddClearBurnCallback(slowDown);
        }
        StartCoroutine(speedCalc());
    }

    private void Start()
    {

    }


    private void FixedUpdate()
    {
        print("CT velocity: " + GetComponent<Rigidbody>().velocity); 

        AutoMove();
        // Strafe is called in update instead of a callback, allows it to update every frame.
        Strafe(); 
        yDeath();
    }

    /// <summary>
    /// Strafe controls based on the strafeSpeed variable using the PlayerInput Input Actions class
    /// </summary>
    private void Strafe()
    {
        // Do not apply input while grappling. Later I may add a special input mode during grapple events.
        GrappleHook grapple = GetComponent<GrappleHook>();
        if(grapple && grapple.GetAttachedTo() != null)
        {
            return;
        }

        Vector2 moveVec = playerController.Controls.Move.ReadValue<Vector2>();
        Vector3 targetAccleration = GetComponent<Rigidbody>().velocity;
        targetAccleration.x = (moveVec.x + moveVec.y) * strafeSpeed;
        InstantaneousAcceleration(targetAccleration);
        
    }

    /// <summary>
    /// Translates the player forward based on the forwardSpeed variable
    /// </summary>
    private void AutoMove()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 inputForce = DirectionalMovement();
        Vector3 autoForce = Vector3.zero;
        float dot = Vector3.Dot(rb.velocity.normalized, inputForce);

        GrappleHook grapple = GetComponent<GrappleHook>();
        if (!(grapple && grapple.IsGrappleActive()))
        {
            autoForce += transform.forward * forwardThrust;

            if (Mathf.Abs(rb.velocity.z) >= maxSpeed * internalBurnSpeedMultiplier && dot >= 0)
            {
                autoForce = transform.forward * rb.mass * ((maxSpeed * internalBurnSpeedMultiplier) - rb.velocity.magnitude) / Time.fixedDeltaTime;
            }
            rb.AddForce((autoForce) * Time.deltaTime);
        }

        if (Mathf.Abs(rb.velocity.z) >= maxSpeed * internalBurnSpeedMultiplier && dot >= 0
            || Mathf.Abs(rb.velocity.z) <= minSpeed * internalBurnSpeedMultiplier && dot <= 0)
        {
            ;
        }
        else
        {
            rb.AddForce((inputForce) * Time.deltaTime);
        }
    }
    

    /// <summary>
    /// Instantly change to target velocity.
    /// </summary>
    /// <param name="targetVelocity"></param>
    private void InstantaneousAcceleration(Vector3 targetVelocity)
    {
        
        Vector3 a = targetVelocity - GetComponent<Rigidbody>().velocity;
        Vector3 F = a * GetComponent<Rigidbody>().mass;

        GetComponent<Rigidbody>().AddForce(F, ForceMode.Acceleration);

        //print("Force: " + a);

        //print("Vel: " + GetComponent<Rigidbody>().velocity);
        
    }
    private Vector3 DirectionalMovement()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 force = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            force += transform.forward * forwardThrust * runThrustScale;
            maxSpeed = maxRunSpeed;
        }
        else
        {
            maxSpeed = maxDefaultSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            force += (-transform.forward * forwardThrust * runThrustScale);
        }
        if (Input.GetKey(KeyCode.A))
        {
            force += (-transform.right * horizontalThrust);
        }
        if (Input.GetKey(KeyCode.D))
        {
            force += (transform.right * horizontalThrust);
        }

        return force;
    }

    

    /// <summary>
    /// Add vertical impulse.
    /// </summary>
    /// <param name="context"></param>
    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump:" + context.phase);
        if (CheckIfGrounded() && context.performed)
        {
            Debug.Log("real Jump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    

    /// <summary>
    /// Check if player is suitably close to ground.
    /// </summary>
    /// <returns></returns>
    public bool CheckIfGrounded()
    {
        Vector3 start = transform.position;
        Vector3 end = start - groundProbeDepth * Vector3.up;

        Debug.DrawLine(start, end, Color.red, 1.0f);
        return Physics.Linecast(start, end);
    }

    public void playerSpeed()
    {
        
        StartCoroutine(speedCalc());

    }

    IEnumerator speedCalc()
    {
        bool isPlaying = true;
        Debug.Log("speed check");
        while (isPlaying)
        {
            //Debug.Log("Speed: " + currentSpeed);
            Vector3 prevPos = transform.position;
            prevPos.x = 0;
            prevPos.y = 0;
            yield return new WaitForFixedUpdate();
            Vector3 currPos = transform.position;
            currPos.x = 0;
            currPos.y = 0;
            currentSpeed = Vector3.Distance(currPos, prevPos) / Time.fixedDeltaTime;
        }
        
    }

    private void speedBoost()
    {
        Debug.Log("BOOOOST");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * boostScale * rb.velocity.magnitude, ForceMode.VelocityChange);

        internalBurnSpeedMultiplier = burnSpeedMultiplier;
        jumpForce = burnJumpForce;

    }

    private void slowDown()
    {
        Debug.Log("slow");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(-rb.velocity.normalized * slowScale * rb.velocity.magnitude, ForceMode.VelocityChange);

        internalBurnSpeedMultiplier = 1;
        jumpForce = baseJumpForce;
    }
   
    public void yDeath()
    {
        if (transform.position.y < -deathPlaneDepth)
        {
            Death();
            print ("Y died!");
        }
    }
    public void Death()
    {
        LevelTransitions levels = FindObjectOfType<LevelTransitions>();
        if (levels != null)
        {
            // reload if we are using scene management   
            levels.ReloadCurrent();
        }
        else
        {
            // otherwise just manually reset.
            PlayerStats_Szolo playerstats = GetComponent<PlayerStats_Szolo>();
            playerstats.ResetStatus();
            playerstats.ResetCollectables();
            //transform.position = spawnPoint;
        }
    }

    private void OnDestroy()
    {
        playerController.Controls.Jump.performed -= Jump;
    }

}
