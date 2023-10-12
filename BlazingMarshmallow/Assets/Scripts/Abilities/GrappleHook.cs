using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    protected Rigidbody rb;

    protected GameObject attachedTo;
    protected Vector3 attachPoint;
    [SerializeField] protected float length = 1;

    [SerializeField] protected GameObject cursor;
    [SerializeField] protected float cursorSensitivity = 1;
    protected GrappleInput input;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();
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

    }

    /// <summary>
    /// Shoot a ray from cursor.
    /// Start applying grapple force if it hit an object in the grapple layer.
    /// </summary>
    protected void Fire()
    {

    }

    /// <summary>
    /// Stop applying grapple force.
    /// </summary>
    protected void Release()
    {

    }

    /// <summary>
    /// Constrain the rigidbody as if a rope of length was tied to attachPoint.
    /// </summary>
    protected void ApplyGrappleForce()
    {

    }
}
