using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : MonoBehaviour
{
    protected Rigidbody rb;

    protected GameObject attachedTo;
    protected Vector3 attachPoint;
    [SerializeField] protected float length = 1;

    [SerializeField] protected GameObject cursor;
    [SerializeField] protected float cursorSensitivity = 1;
    protected GrappleInput input;

    bool b = false;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        input.Grapple.Fire.started += Fire;
    }

    void Update()
    {
        SwivelCursor();
    }

    /// <summary>
    /// Translate the cursor represented by a canvas object.
    /// </summary>
    protected void SwivelCursor()
    {
        Vector3 offset = input.Grapple.SwivelCursor.ReadValue<Vector2>();
        if(offset.magnitude > 1) { offset.Normalize(); }
        offset *= cursorSensitivity;
        cursor.transform.position += new Vector3(offset.x, offset.y, 0);
    }

    /// <summary>
    /// Shoot a ray from cursor.
    /// Start applying grapple force if it hit an object in the grapple layer.
    /// </summary>
    protected void Fire(InputAction.CallbackContext context)
    {
        print("Fire");

        Vector3 viewPos = Camera.main.WorldToViewportPoint(cursor.transform.position);
        Ray ray = Camera.main.ViewportPointToRay(viewPos);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 999, Color.red, 10.0f);
        //Debug.DrawLine(viewPos, viewPos + cursor.transform.forward * 999, Color.yellow, 10);
        if(Physics.Raycast(ray, out hit, 999))
        {
            if(hit.collider.gameObject.layer == 3)
            {
                print("Hit Grapple Obj");
            }
        }
    }

    /// <summary>
    /// Stop applying grapple force.
    /// </summary>
    protected void Release(InputAction.CallbackContext context)
    {

    }

    /// <summary>
    /// Constrain the rigidbody as if a rope of length was tied to attachPoint.
    /// </summary>
    protected void ApplyGrappleForce()
    {

    }
}
