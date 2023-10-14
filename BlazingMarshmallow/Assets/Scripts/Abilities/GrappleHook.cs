using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GrappleHook : MonoBehaviour
{
    protected Rigidbody rb;

    protected GameObject attachedTo;
    protected Vector3 attachPoint;
    [SerializeField] protected float length = 1;
    [SerializeField] protected float range = 1;
    [SerializeField] protected float boost = 0;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float exitImpulse = 2;

    [SerializeField] protected GameObject cursor;
    [SerializeField] protected float cursorSensitivity = 1;
    protected GrappleInput input;

    float screen_x;
    float screen_y;

    Vector3 initialVelocity;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();

        Vector3 bounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 4));
        screen_x = Mathf.Abs(Camera.main.transform.position.x - bounds.x);  
        screen_y = Mathf.Abs(Camera.main.transform.position.y - bounds.y);  

        print("Cam Bounds: " + screen_x + " : " + screen_y);

        StartCoroutine("SwivelCursor");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        input.Grapple.Fire.started += Fire;
        input.Grapple.Fire.canceled += Release;
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
            cursor.transform.localPosition += new Vector3(offset.x, offset.y, 0);

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

        Debug.DrawRay(ray.origin, ray.direction * 999, Color.red, 10.0f);
        if(Physics.Raycast(ray, out hit, 999, mask))
        {
            if((transform.position - hit.point).magnitude <= range)
            {
                print("Hit Grapple Obj");
                attachedTo = hit.collider.gameObject;
                attachPoint = hit.point;
     
                initialVelocity = rb.velocity;
                
                StartCoroutine(InterpCoroutine());
                
                print("Range: " + range + " Length: " + length);
            }
        }
    }

    /// <summary>
    /// Stop applying grapple force.
    /// </summary>
    protected void Release(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody>().AddForce(GetFlingDirection() * exitImpulse, ForceMode.Impulse);
        attachedTo = null;
        StopCoroutine(ApplyGrappleForce());
    }

    /// <summary>
    /// Constrain the rigidbody as if a rope of length was tied to attachPoint.
    /// </summary>
    protected IEnumerator ApplyGrappleForce()
    {
        float grappleSeconds = 0;
        float secondsUnderTension = 0;

        while (attachedTo != null)
        {
            grappleSeconds += Time.fixedDeltaTime;
            
            Vector3 rope = attachPoint - transform.position;
            float theta = Mathf.Acos(Vector3.Dot(Vector3.up, rope.normalized));

            print("Theta: " + theta);

            Debug.DrawLine(attachPoint, attachPoint - rope.normalized * length, Color.blue, .1f);
            if(rope.magnitude > length)
            {
                secondsUnderTension += Time.fixedDeltaTime; 

                transform.position = attachPoint - rope.normalized * length;
                
                float tension = Mathf.Abs(rb.mass * Mathf.Pow(rb.velocity.magnitude, 2) / length + rb.mass * Physics.gravity.magnitude * Mathf.Cos(theta));

                // Tension force.
                rb.AddForce(tension * rope.normalized);
                
                // Boost force.
                rb.AddForce(GetFlingDirection() / (grappleSeconds * grappleSeconds) * boost * length);

                // Naive drag.
                rb.AddForce(-GetFlingDirection());
            }
            else
            {
                secondsUnderTension = 0;
            }

            print("Velocity: " + rb.velocity);

            yield return new WaitForFixedUpdate();
        }
    }

    protected IEnumerator InterpCoroutine()
    {
        while((transform.position - GetTargetPosition()).magnitude > .5f)
        {
            print("Interp");
            transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), Time.deltaTime * interpSpeed);
            yield return new WaitForEndOfFrame();
        }

        Vector3 v = initialVelocity.magnitude * GetFlingDirection();
        rb.velocity = v;

        StartCoroutine(ApplyGrappleForce());
    }


    public Vector3 GetFlingDirection()
    {
        Vector3 flingDirection = Vector3.Cross(transform.right, attachPoint - transform.position).normalized;
        int direction = Vector3.Dot(flingDirection, rb.velocity.normalized) > 0 ? 1 : -1;
        flingDirection *= direction;
        return flingDirection;
    }

    public Vector3 GetTargetPosition()
    {
        return attachPoint - (attachPoint - transform.position).normalized * length;
    }
}
