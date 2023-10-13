using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

//Kriger Nathan

public class Controller : MonoBehaviour
{
    public float forwwardSpeed = 5f;
    public float strafeSpeed = 5f;
    public float jumpForce = 5f;
    public float groundProbeDepth = 1.0f;
    
    //define a reference to our input actions.
    private PlayerInput playerController;

    private void Awake()
    {
        // Initialize our input actions.
        playerController = new PlayerInput();
        playerController.Enable();

        // Add jump callback.
        playerController.Controls.Jump.performed += Jump;
    }
   
    private void FixedUpdate()
    {
        AutoMove();
        // Strafe is called in update instead of a callback, allows it to update every frame.
        Strafe(); 
    }

    //Strafe controls based on the strafeSpeed variable using the PlayerInput Input Actions class
    private void Strafe()
    {
        Vector2 moveVec = playerController.Controls.Move.ReadValue<Vector2>();
        if(CheckIfGrounded() || moveVec.magnitude > 0)
        {
            Vector3 targetAccleration = GetComponent<Rigidbody>().velocity;
            targetAccleration.x = (moveVec.x + moveVec.y) * strafeSpeed;
            InstantaneousAcceleration(targetAccleration);
        }

        //transform.Translate(new Vector3(moveVec.x, 0, moveVec.y) * Time.deltaTime * strafeSpeed);
    }

    //a function that translates the player forword based on the forwardSpeed variable
    private void AutoMove()
    {
        if(CheckIfGrounded())
        {
            Vector3 targetVelocity = GetComponent<Rigidbody>().velocity;
            targetVelocity.z = forwwardSpeed;
            InstantaneousAcceleration(targetVelocity);
        }
    }

    private void InstantaneousAcceleration(Vector3 targetVelocity)
    {
        Vector3 a = targetVelocity - GetComponent<Rigidbody>().velocity;
        Vector3 F = a * GetComponent<Rigidbody>().mass;

        GetComponent<Rigidbody>().AddForce(F, ForceMode.Impulse);

        print("Force: " + a);

        print("Vel: " + GetComponent<Rigidbody>().velocity);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump:" + context.phase);
        if (CheckIfGrounded() && context.performed)
        {
            Debug.Log("real Jump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool CheckIfGrounded()
    {
        Vector3 start = transform.position;
        Vector3 end = start - groundProbeDepth * Vector3.up;

        Debug.DrawLine(start, end, Color.red, 1.0f);
        return Physics.Linecast(start, end);
    }

    private void BurningSpeed()
    {
        forwwardSpeed = 8f;
        strafeSpeed = 8f;
        jumpForce = 8f;
    }
}
