using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchGrapple : MonoBehaviour
{
    public GrappleHook grapple;

    public Vector3 grapplePoint = Vector3.zero;
    public Vector3 testOffset = Vector3.zero;

    Vector3 baseScale = Vector3.one;
    Quaternion baseRotation = Quaternion.identity;

    protected void Start()
    {
        baseScale = transform.localScale;
        baseRotation = transform.rotation;
    }

    protected void Update()
    {
        if(grapple != null && grapple.IsGrappleActive())
        {
            testOffset = grapple.GetAttachPoint() - transform.position;


            print("SR rotate head");
            RotateHead();
            Stretch();
        }
        else
        {
            transform.localScale = baseScale;
            transform.rotation = baseRotation;
        }

    }

    protected void RotateHead()
    {
        grapplePoint = transform.position + testOffset;
        print("SR rotate head");

        Vector3 diff = (grapplePoint - transform.position).normalized;
        float angle = Vector3.Angle(Vector3.up, diff);
        Vector3 axis = Vector3.Cross(Vector3.up, diff).normalized;

        Quaternion newRot = Quaternion.AngleAxis(angle, axis);

        transform.rotation = newRot;
    }

    protected void Stretch()
    {
        Vector3 diff = (testOffset);
        float length = diff.magnitude;

        print("SR loc scale " + baseScale);

        Vector3 scale = baseScale;
        scale.y *= length / 2;
        transform.localScale = scale;   
    }
}
