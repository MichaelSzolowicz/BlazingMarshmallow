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
    protected GrappleInput input;

    protected GameObject attachedTo;
    protected Vector3 attachPoint;
    [SerializeField] protected float length = 10;
    protected float internalLength = 10;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float boost = 0f;
    [SerializeField] protected float dampeningScale = 2;

    [Header("===== Exit parameters =====")]
    [SerializeField, Tooltip("Raw vertical impulse applied on exit.")] 
    protected float exitJump = 2;
    [SerializeField, Tooltip("Impulse applied against vertical velocity, scaled to vertical component of arc tangent.")] 
    protected float exitVerticalDampening = 22;
    [SerializeField, Tooltip("Extra forward impulse applied on exit, proportional to vertical component of arc tangent.")] 
    protected float exitForwardImpulse = 10;

    [Header("===== Aiming =====")]
    [SerializeField] protected Aiming aiming;

    [Header("===== Line renderer =====")]
    public LineRenderer lineRenderer;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();

        //input.Grapple.Fire.started += Fire;
        input.Grapple.Fire.canceled += Release;

        internalLength = length;

        if(aiming != null)
        {
            StartCoroutine(ScanAndFire());
        }
    }

    /// <summary>
    /// Probe the scene for grapple objects.
    /// Attach if the player pressed the grapple button.
    /// </summary>
    private IEnumerator ScanAndFire()
    {
        while(true)
        {
            RaycastHit hit;
            aiming.Scan(out hit);

            if (!IsGrappleActive() && hit.collider != null)
            {
                if (input.Grapple.Fire.WasPerformedThisFrame())
                {
                    attachPoint = hit.point;
                    attachedTo = hit.collider.gameObject;

                    UpdateLineRenderer();

                    initialVelocity = rb.velocity;

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grapple"))
                    {
                        StartCoroutine(ApplyGrappleForce());
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Claw"))
                    {
                        StartCoroutine(InterpClaw());
                    }
                }
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
        StopCoroutine(InterpSwing());
        if (attachedTo != null && attachedTo.gameObject.layer != LayerMask.NameToLayer("Claw"))
        {
            ExitImpulse();
        }
        rb.WakeUp();
        attachedTo = null;
        internalLength = length;
        UpdateLineRenderer();
    }

    protected void ExitImpulse()
    {
        Vector3 impulse = Vector3.up;
        impulse *= -exitVerticalDampening * GetFlingDirection().y;
        impulse += transform.up * exitJump;
        impulse += transform.forward * exitForwardImpulse * Mathf.Abs(GetFlingDirection().y);

        Debug.Log("Exit impulse: " + impulse.ToString());

        GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
    }

    /// <summary>
    /// Constrain the rigidbody as if a rope of length was tied to attachPoint.
    /// </summary>
    protected IEnumerator ApplyGrappleForce()
    {
        StartCoroutine(InterpSwing());

        float grappleSeconds = 0;
        float secondsUnderTension = 0;

        while (attachedTo != null)
        {
            grappleSeconds += Time.fixedDeltaTime;

            UpdateLineRenderer();
            
            Vector3 rope = attachPoint - transform.position;
            float theta = Mathf.Acos(Vector3.Dot(Vector3.up, rope.normalized));

            print("Theta: " + theta);

            Debug.DrawLine(attachPoint - rope.normalized * internalLength, (attachPoint - rope.normalized * internalLength) + GetFlingDirection() * 5, Color.green, 0.1f);
            Debug.DrawLine(attachPoint, attachPoint - rope.normalized * internalLength, Color.blue, .1f);
            if(rope.magnitude > internalLength || isInterpolating)
            {
                secondsUnderTension += Time.fixedDeltaTime; 

                transform.position = attachPoint - rope.normalized * internalLength;
                
                float tension = Mathf.Abs(rb.mass * Mathf.Pow(rb.velocity.magnitude, 2) / internalLength + rb.mass * Physics.gravity.magnitude * Mathf.Cos(theta));

                // Tension force.
                rb.AddForce(tension * rope.normalized);
                
                // Boost force.
                rb.AddForce(GetFlingDirection() * boost * internalLength / (grappleSeconds * grappleSeconds));

                // Naive drag.
                rb.AddForce(-rb.velocity.normalized * dampeningScale * (1 + rb.velocity.magnitude) / internalLength);
            }
            else
            {
                secondsUnderTension = 0;
            }

            print("Velocity: " + rb.velocity);

            yield return new WaitForFixedUpdate();
        }
    }


    protected IEnumerator InterpClaw()
    {
        isInterpolating = true;
        internalLength = 0;
        StartCoroutine(ApplyGrappleForce());

        while(IsGrappleActive())
        {
            rb.velocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }

        isInterpolating = false;
        rb.velocity = initialVelocity;
        yield return null;
    }

    

    /// <summary>
    /// Interp to length away from the attachedTo object.
    /// Used to help make initial grab ons more consistent.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator InterpSwing()
    {
        isInterpolating = true;

        Vector3 rope = attachPoint - transform.position;
        float ropeLength = rope.magnitude;
        float targetLength = internalLength;
        internalLength = ropeLength;

        print("Interp " + Mathf.Abs(targetLength - length));

        rb.velocity = rb.velocity.magnitude * GetFlingDirection();

        // perform the interpolation
        while (Mathf.Abs(targetLength - internalLength) > .1f)
        {
            internalLength = Mathf.Lerp(internalLength, targetLength, Time.fixedDeltaTime * interpSpeed);

            yield return new WaitForFixedUpdate();  
        }

        print("GP end grapple. targeted length: " + targetLength);

        internalLength = length;
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
