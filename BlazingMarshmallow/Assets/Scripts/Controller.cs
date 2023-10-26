using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

//Kriger Nathan & Michael Szolowicz

public class Controller : MonoBehaviour
{
    [Header("Movement Speed")]
    public float forwwardSpeed = 5f;
    public float strafeSpeed = 5f;
    public float jumpForce = 5f;
    public float groundProbeDepth = 2f;

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
    public float forwardThrust;


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
        SetResetSpeed();
        spawnPoint = transform.position;
    }


    private void FixedUpdate()
    {
        AutoMove();
        // Strafe is called in update instead of a callback, allows it to update every frame.
        Strafe(); 
	    burnCheck();
        Death();
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
        DirectionalMovement();
        // Only auto move when grounded. Velcoity maintained when leaving ground.
        if (CheckIfGrounded())
        {
            Vector3 targetVelocity = GetComponent<Rigidbody>().velocity;
            targetVelocity.z = forwwardSpeed;
            InstantaneousAcceleration(targetVelocity);
            
        }
       
    }
    private void DirectionalMovement()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * forwardThrust * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * forwardThrust * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * horizontalThrust * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * horizontalThrust * Time.deltaTime);
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

        print("Force: " + a);

        print("Vel: " + GetComponent<Rigidbody>().velocity);
        
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
        }
        else
        {
            Debug.Log("Not Grounded");
            rb.drag = 0;
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

    public void BurningSpeed()
    {
       forwwardSpeed = burnSpeed;
       strafeSpeed = burnStrafeSpeed;
       jumpForce = burnJumpForce;
    }

    public void ResetSpeed()
    {
        forwwardSpeed = resetSpeed;
        strafeSpeed = resetStrafeSpeed;
        jumpForce = resetJumpForce;
    }

    private void burnCheck()
    {
        PlayerStats_Szolo playerStats = GetComponent<PlayerStats_Szolo>();
        if (playerStats.currentStatus == PlayerStats_Szolo.Status.Burned)
        {
            BurningSpeed();
        }
        else
        {
          ResetSpeed();
        }

    }
    public void SetResetSpeed()
    {
       resetJumpForce = jumpForce;
       resetSpeed = forwwardSpeed;
       resetStrafeSpeed = strafeSpeed;
    }
   
    public void Death()
    {
        if (transform.position.y < -10)
        {
            ResetSpeed();
            PlayerStats_Szolo playerstats = GetComponent<PlayerStats_Szolo>();
            playerstats.ResetStatus();
            playerstats.ResetCollectables();
            transform.position = spawnPoint;
        }
    }

}
