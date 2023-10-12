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

    float screen_x;
    float screen_y;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();

        Vector3 lower_bounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, cursor.transform.position.z));
        screen_x = lower_bounds.x;  
        screen_y = lower_bounds.y;  

        StartCoroutine("SwivelCursor");


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        input.Grapple.Fire.started += Fire;
    }

    void Update()
    {

    }

    /// <summary>
    /// Translate the cursor represented by a canvas object.
    /// </summary>
    protected IEnumerator SwivelCursor()
    {
        while(true)
        {
            Vector3 offset = input.Grapple.SwivelCursor.ReadValue<Vector2>();
            offset *= cursorSensitivity * Time.deltaTime;
            cursor.transform.position += new Vector3(offset.x, offset.y, 0);

            Vector3 correction = cursor.transform.localPosition;
            if (cursor.transform.localPosition.x > screen_x)
            {
                correction.x = screen_x;
                cursor.transform.localPosition = correction;
            }
            if (cursor.transform.localPosition.x < -screen_x)
            {
                correction.x = -screen_x;
                cursor.transform.localPosition = correction;
            }
            if (cursor.transform.localPosition.y > screen_y)
            {
                correction.y = screen_y;
                cursor.transform.localPosition = correction;
            }
            if (cursor.transform.localPosition.y < -screen_y)
            {
                correction.y = -screen_y;
                cursor.transform.localPosition = correction;
            }

            yield return new WaitForEndOfFrame();
        }
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
        int mask = LayerMask.GetMask("Grapple");

        Debug.DrawRay(ray.origin, ray.direction * length, Color.red, 10.0f);
        if(Physics.Raycast(ray, out hit, length, mask))
        {
             print("Hit Grapple Obj");
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
