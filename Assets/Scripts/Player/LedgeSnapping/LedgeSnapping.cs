using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LedgeSnapping : MonoBehaviour
{
    protected Rigidbody rb;

    [SerializeField] protected float fallSpeedCutoff = 10;
    [SerializeField] protected float threshold = .5f;
    [SerializeField] protected float maxSnapHeight = 3;
    [SerializeField] protected float forwardProbeDistance = 1.5f;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float characterHeight = 2;
    [SerializeField] protected float characterRadius = .5f;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;
    protected Vector3 lastFrameVelocity;
    protected Vector3 lastPosition;

    Vector3 previousCollisionImpulse = Vector3.zero;
    public float dotThresh = .7f;
    public float magThresh = 5f;

    private void Update()
    {
        //print("LS velocity: " + rb.velocity);
        lastFrameVelocity = rb.velocity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("LS collision");

        List<ContactPoint> contacts = new List<ContactPoint>();
        int num = collision.GetContacts(contacts);

        Vector3 avgPoint = Vector3.zero;
        Vector3 avgNormal = Vector3.zero;

        foreach(ContactPoint contact in contacts)
        {
            //print("LS draw point: " + contact.point);

            avgPoint += contact.point;
            avgNormal += contact.normal;    
        }

        avgNormal /= num;
        avgPoint /= num;

        Debug.DrawLine(avgPoint, avgPoint + avgNormal * 500, Color.blue, 100.0f);


        print("LS Impulse: " + collision.impulse + ", mag:  " + collision.impulse.magnitude + ", dot: " + Vector3.Dot(previousCollisionImpulse.normalized, collision.impulse.normalized) + ", obj: " + collision.gameObject.name);

        print("LS impulse dist: " + Vector3.Distance(lastPosition, transform.position));
        


        //float dot = Vector3.Dot(avgNormal, Vector3.up);
        //float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        //float velocityResistance = Mathf.Abs(Vector3.Dot(avgNormal, lastFrameVelocity.normalized));

        if (avgPoint.y > (transform.position.y - characterHeight / 2))
        {
            print("LS avgNormal: " + avgNormal + ", rbVelocity: " + lastFrameVelocity + ", Obj: " + collision.gameObject.name);
            if (!isInterpolating)
            {
                initialVelocity = lastFrameVelocity;
                //print("LS set init vel: " + initialVelocity);
            }
            SnapUp(); 
        }

        previousCollisionImpulse = collision.impulse;
        lastPosition = rb.position;
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
        capsule.enabled = false;
        rb.useGravity = false;
        targetPos += Vector3.up * .1f;

        while((targetPos.y) - transform.position.y >= .1f)
        {
            //print("LS interping...");

            Vector3 point1 = transform.position - (transform.up * (capsule.height / 2 - capsule.radius));
            Vector3 point2 = point1 + (transform.up * (capsule.height - capsule.radius));
            Vector3 delta = Vector3.LerpUnclamped(transform.position, targetPos, Time.fixedDeltaTime * interpSpeed) - transform.position;
            RaycastHit hit;

            Debug.DrawLine(point1, point2, Color.magenta, 100);
            Debug.DrawLine(point1 + delta, point2 + delta, Color.magenta, 100);

            if (Physics.CapsuleCast(point1, point2, capsule.radius, delta.normalized, out hit, delta.magnitude))
            {
                //print("LS capsule hit");

                delta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * delta.magnitude;
            }

            delta.x = 0;
            delta.z = 0;
            transform.position += delta;

            yield return new WaitForFixedUpdate();
        }

        rb.useGravity = true;
        yield return new WaitForFixedUpdate();

        capsule.enabled = true;
        isInterpolating = false;
        //rb.velocity = initialVelocity;
        print("LS interp end. Velocity: " + initialVelocity);
    }

    public bool GetIsInterpolating()
    {
        return isInterpolating;
    }
}
