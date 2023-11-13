using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

//Kriger Nathan & Michael Szolowicz

public class Controller : MonoBehaviour
{
    [Header("Movement Defaults")]
    public float forwardThrust = 1000f;
    public float runThrustScale = 2f;
    public float maxDefaultSpeed = 50f;
    public float maxRunSpeed = 80f;
    public float minSpeed = 0;
    public float strafeSpeed = 5f;
    public float burnSpeedMultiplier = 1.5f;
    private float maxSpeed;
    private float internalBurnSpeedMultiplier;
    
    
    [Header("Movement Speed")]
    public float forwwardSpeed = 5f;
    public float maxDefaultGroundSpeed = 50f;
    public float airSpeed;

    public float jumpForce = 5f;
    public float groundProbeDepth = 2f;
    public float Boost = 5;
    public float Slow = 2;

    private Vector3 spawnPoint;
    
    [Header("Burned Speed")]
    public float burnSpeed = 8f;
    private float burnStrafeSpeed = 8f;
    private float burnJumpForce = 8f;

    [SerializeField]
    private float resetSpeed =5f;
    private float resetStrafeSpeed = 5f;
    private float resetJumpForce = 5f;

    //define a reference to our input actions.
    private PlayerInput playerController;

    [Header("Grounded Check")]
    public float groundDrag = 5.0f;
    public LayerMask Ground;
    private bool grounded = false;

    [Header("Directional Movement")]
    public float horizontalThrust;

    public float currentSpeed;
    public float playerHeight = 1;

    private void Awake()
    {
        // Initialize our input actions.
        playerController = new PlayerInput();
        playerController.Enable();

        // Add jump callback.
        playerController.Controls.Jump.performed += Jump;
    }

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * minSpeed;
        maxSpeed = maxDefaultGroundSpeed;
        internalBurnSpeedMultiplier = 1;
        SetResetSpeed();
        spawnPoint = transform.position;
        GrappleHook grapple = GetComponent<GrappleHook>();
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();
        if(playerStats != null ) 
        {
            playerStats.AddInflictBurnCallback(speedBoost);
            playerStats.AddClearBurnCallback(slowDown);
        }
        StartCoroutine(speedCalc());
    }


    private void FixedUpdate()
    {
        print("CT velocity: " + GetComponent<Rigidbody>().velocity); 

        AutoMove();
        // Strafe is called in update instead of a callback, allows it to update every frame.
        Strafe(); 
        yDeath();
        groundedConfirm();
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
                ;
            }
            else
            {
                rb.AddForce((autoForce) * Time.deltaTime);
            }
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

    private void groundedConfirm()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        Rigidbody rb = GetComponent<Rigidbody>();

        if (grounded)
        {
            rb.drag = groundDrag;
            Debug.Log("Grounded");
            speedLimiter();
            GrappleHook grapple = GetComponent<GrappleHook>();
            grapple.isGrappling = false;
        }
        else
        {
            Debug.Log("Not Grounded");
            rb.drag = 0;
        }
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
            yield return new WaitForFixedUpdate();
            currentSpeed = Mathf.RoundToInt(Vector3.Distance(transform.position, prevPos) / Time.fixedDeltaTime);
        }
        
    }
   
    private void speedLimiter()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > forwwardSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwwardSpeed);
        }
    }
    //Collide and Slide Testing to get better movement mechanics

    

    private void speedBoost()
    {
        Debug.Log("BOOOOST");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(rb.velocity.normalized * Boost * rb.velocity.magnitude, ForceMode.VelocityChange);

        internalBurnSpeedMultiplier = burnSpeedMultiplier;
    }

    private void slowDown()
    {
        Debug.Log("slow");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(-rb.velocity.normalized * Slow * rb.velocity.magnitude, ForceMode.VelocityChange);

        internalBurnSpeedMultiplier = 1;
    }

    public void SetResetSpeed()
    {
       resetJumpForce = jumpForce;
       resetSpeed = forwwardSpeed;
       resetStrafeSpeed = strafeSpeed;
    }
   
    public void yDeath()
    {
        if (transform.position.y < -10)
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
