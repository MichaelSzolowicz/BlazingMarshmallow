using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UI;


/// <summary>
/// [Michael Szolowicz]
/// Defines behavior of a grapple hook acting on a rigidbody.
/// </summary>

public class GrappleHook : MonoBehaviour
{
    protected Rigidbody rb;

    protected GameObject attachedTo;
    protected Vector3 attachPoint;
    [SerializeField] protected float length = 1;
    [SerializeField] protected float range = 1;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float boost = 0;
    [SerializeField] protected float dampeningScale = 1;
    [SerializeField] protected float exitImpulse = 2;

    [SerializeField] protected GameObject cursor;
    [SerializeField] protected float cursorSensitivity = 1;
    protected bool bGrappleObjInRange;
    protected GrappleInput input;

    

    protected float screen_x;
    protected float screen_y;

    public LineRenderer lineRenderer;
    public Image screenCursor;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;

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

        //input.Grapple.Fire.started += Fire;
        input.Grapple.Fire.canceled += Release;
    }

    private void Update()
    {
        ScanAndFire();
    }

    /// <summary>
    /// Probe the scene for grapple objects.
    /// Attach if the player pressed the grapple button.
    /// </summary>
    private void ScanAndFire()
    {
        
        Vector3 viewPos = Camera.main.WorldToViewportPoint(cursor.transform.position);
        Ray ray = Camera.main.ViewportPointToRay(viewPos);
        RaycastHit hit;
        string[] layers = { "Grapple", "Claw" };
        int mask = LayerMask.GetMask(layers);

        //Debug.DrawRay(ray.origin, ray.direction * 999, Color.red, 10.0f);
        if (Physics.Raycast(ray, out hit, 999, mask))
        {
            if ((transform.position - hit.point).magnitude <= range)
            {
                bGrappleObjInRange = true;
                if (screenCursor) screenCursor.color = Color.green;

                if (input.Grapple.Fire.WasPerformedThisFrame())
                {
                    attachPoint = hit.point;
                    attachedTo = hit.collider.gameObject;

                    UpdateLineRenderer();

                    initialVelocity = rb.velocity;

                    StartCoroutine(InterpCoroutine());
                    
                }
            }
        }
        else
        {
            bGrappleObjInRange = false;
            if (screenCursor) screenCursor.color = Color.white;
        }
    }


    /// <summary>
    /// Translate the cursor represented by a canvas object.
    /// </summary>
    protected IEnumerator SwivelCursor()
    {
        // Skip the first frame since it usually has a large offset.
        yield return null;

        while(true)
        {
            Vector3 offset = input.Grapple.SwivelCursor.ReadValue<Vector2>();
            offset *= cursorSensitivity;
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

            yield return null;
        }
    }

    /// <summary>
    /// Stop applying grapple force.
    /// </summary>
    protected void Release(InputAction.CallbackContext context)
    {
        StopCoroutine(ApplyGrappleForce());
        if (attachedTo != null)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * exitImpulse, ForceMode.Impulse);
        }
        attachedTo = null;
        UpdateLineRenderer();
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

            UpdateLineRenderer();
            
            Vector3 rope = attachPoint - transform.position;
            float theta = Mathf.Acos(Vector3.Dot(Vector3.up, rope.normalized));

            print("Theta: " + theta);

            Debug.DrawLine(attachPoint - rope.normalized * length, (attachPoint - rope.normalized * length) + GetFlingDirection() * 5, Color.green, 0.1f);
            Debug.DrawLine(attachPoint, attachPoint - rope.normalized * length, Color.blue, .1f);
            if(rope.magnitude > length)
            {
                secondsUnderTension += Time.fixedDeltaTime; 

                transform.position = attachPoint - rope.normalized * length;
                
                float tension = Mathf.Abs(rb.mass * Mathf.Pow(rb.velocity.magnitude, 2) / length + rb.mass * Physics.gravity.magnitude * Mathf.Cos(theta));

                // Tension force.
                rb.AddForce(tension * rope.normalized);
                
                // Boost force.
                rb.AddForce(GetFlingDirection() * boost * length / (grappleSeconds * grappleSeconds));

                // Naive drag.
                rb.AddForce(-rb.velocity.normalized * dampeningScale * (1 + rb.velocity.magnitude) / length);
            }
            else
            {
                secondsUnderTension = 0;
            }

            print("Velocity: " + rb.velocity);

            yield return new WaitForFixedUpdate();
        }
    }

    

    /// <summary>
    /// Interp to length away from the attachedTo object.
    /// Used to help make initial grab ons more consistent.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator InterpCoroutine()
    {
        isInterpolating = true;

        // Are using claw or swing? if latter, set interp point to length away from the actual grapple point.
        Vector3 interpPoint;
        if (attachedTo.layer == LayerMask.NameToLayer("Grapple"))
        {
            interpPoint = GetTargetPosition();
        }
        else
        {
            interpPoint = attachPoint;
        }

        // perform the interpolation
        while ((transform.position - interpPoint).magnitude - .5f > .5f && input.Grapple.Fire.IsPressed())
        {
            print("Interp");

            if (attachedTo.layer == LayerMask.NameToLayer("Grapple"))
            {
                interpPoint = GetTargetPosition();
            }

            UpdateLineRenderer();

            if(attachedTo && attachedTo.layer == LayerMask.NameToLayer("Claw")) rb.velocity = Vector3.zero;

            transform.position = Vector3.Lerp(transform.position, interpPoint, Time.deltaTime * interpSpeed);
            yield return new WaitForFixedUpdate();
        }

        // Are we swinging? If so redirect velocity along arc curve.
        Vector3 v = initialVelocity;
        if(attachedTo && attachedTo.layer == LayerMask.NameToLayer("Grapple"))
        {
            v = v.magnitude * GetFlingDirection();
            StartCoroutine(ApplyGrappleForce());
        }

        rb.velocity = v;
        isInterpolating = false;
    }

    protected void UpdateLineRenderer()
    {
        if(lineRenderer == null)
        {
            return;
        }

        if (attachedTo != null)
        {
            print("Update Line");

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, attachPoint);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

    }

    /// <summary>
    /// Assuming tensions is applied, this is the direction we would fly when it is released.
    /// Note that tensions does not necessarily need to be applied, this will still return as if tension was being applied.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFlingDirection()
    {
        Vector3 flingDirection = Vector3.Cross(rb.velocity, (transform.position - attachPoint)).normalized;
        flingDirection = Quaternion.AngleAxis(90, (transform.position - attachPoint).normalized) * flingDirection;

        Debug.DrawLine(transform.position, transform.position + flingDirection * 5, Color.magenta, 0.1f);

        int direction = Vector3.Dot(flingDirection, rb.velocity.normalized) > 0 ? 1 : -1;
        flingDirection *= direction;
        return flingDirection;
    }

    /// <summary>
    /// Point length away from attachPoint on the line going through this object.
    /// Used to help make initial grab ons more consistent.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTargetPosition()
    {
        return attachPoint - (attachPoint - transform.position).normalized * length;
    }

    public GameObject GetAttachedTo()
    {
        return attachedTo;
    }

    public bool IsGrappleActive()
    {
        return attachedTo != null;
    }
}
