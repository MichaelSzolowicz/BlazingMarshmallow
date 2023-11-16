using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchGrapple : MonoBehaviour
{
    public Vector3 grapplePoint = Vector3.zero;
    public Vector3 testOffset = Vector3.zero;

    Vector3 baseScale = Vector3.one;

    protected void Start()
    {
        baseScale = transform.localScale;
    }

    protected void Update()
    {
 

        print("SR rotate head");
        RotateHead();
        Stretch();
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
        scale.y *= length;
        transform.localScale = scale;   
    }
}
