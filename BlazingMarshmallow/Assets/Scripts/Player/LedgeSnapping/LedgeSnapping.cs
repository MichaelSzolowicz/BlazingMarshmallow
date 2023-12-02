using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class LedgeSnapping : MonoBehaviour
{
    protected Rigidbody rb;

    [SerializeField] protected float maxSnapHeight = 3;
    [SerializeField] protected float forwardProbeDistance = 1.5f;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float characterHeight = 2;
    [SerializeField] protected float characterRadius = .5f;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;
    public float slopeGradientThreshold = 80.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Scan();
    }

    void Scan()
    {
        Vector3 point1 = transform.position;
        point1.y += characterHeight / 2 + characterRadius;
        Vector3 point2 = transform.position;
        point2.y -= characterHeight / 2 + characterRadius;
        Vector3 direction = rb.velocity * Time.fixedDeltaTime;
        RaycastHit hit = new RaycastHit();
        string[] mask = { "Ground" };
        
        Debug.DrawLine(point1, point2, Color.magenta, Time.fixedDeltaTime);
        Debug.DrawLine(transform.position,  transform.position + direction, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(transform.position - transform.forward * characterRadius, transform.position + transform.forward * characterRadius, Color.magenta, Time.fixedDeltaTime);

        if(Physics.CapsuleCast(point1, point2, characterRadius / 2, direction.normalized, out hit, direction.magnitude, LayerMask.GetMask(mask)))
        {
            print("LS hit" + hit.collider.gameObject.name);

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if(angle > slopeGradientThreshold)
            {
                initialVelocity = rb.velocity;
                SnapUp();
            }
        }
    }

    private void SnapUp()
    {
        Vector3 end = transform.position + (transform.forward * forwardProbeDistance) - (transform.up * characterHeight / 2);
        Vector3 start = end + transform.up * maxSnapHeight;
        RaycastHit hit;

        Debug.DrawLine(start, end, Color.green, 100);

        if(!isInterpolating && Physics.Linecast(start, end, out hit))
        {
            Vector3 t = initialVelocity;
            t.y = 0;
            rb.velocity = t;

            StartCoroutine(InterpCoroutine(hit.point + transform.up * characterHeight / 2));
        }
    }


    private IEnumerator InterpCoroutine(Vector3 targetPos)
    { 
        isInterpolating = true;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        rb.useGravity = false;
        targetPos += Vector3.up * .1f;

        while((targetPos.y) - transform.position.y >= .1f)
        {
            Vector3 point1 = transform.position - (transform.up * (capsule.height / 2 - capsule.radius));
            Vector3 point2 = point1 + (transform.up * (capsule.height - capsule.radius));
            Vector3 delta = Vector3.LerpUnclamped(transform.position, targetPos, Time.fixedDeltaTime * interpSpeed) - transform.position;
            RaycastHit hit;

            Debug.DrawLine(point1, point2, Color.magenta, 100);
            Debug.DrawLine(point1 + delta, point2 + delta, Color.magenta, 100);

            if (Physics.CapsuleCast(point1, point2, capsule.radius, delta.normalized, out hit, delta.magnitude))
            {
                delta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * delta.magnitude;
            }

            transform.position += delta;

            yield return new WaitForFixedUpdate();
        }

        rb.useGravity = true;
        yield return new WaitForFixedUpdate();

        capsule.enabled = true;
        isInterpolating = false;
        rb.velocity = initialVelocity;
        print("LS interp end. Velocity: " + initialVelocity);
    }

    public bool GetIsInterpolating()
    {
        return isInterpolating;
    }
}
