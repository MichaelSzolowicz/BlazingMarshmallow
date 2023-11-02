using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LedgeSnapping : MonoBehaviour
{
    protected Rigidbody rb;

    [SerializeField] protected float minSlopeGradation = 45;
    [SerializeField] protected float maxSnapHeight = 3;
    [SerializeField] protected float forwardProbeDistance = 1.5f;
    [SerializeField] protected float interpSpeed = 10;
    [SerializeField] protected float characterHeight = 2;
    [SerializeField] protected float characterRadius = .5f;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;
    protected Vector3 lastFrameVelocity;

    protected BoxCollider boundingCollider;


    private void Update()
    {
        print("LS velocity: " + rb.velocity);
        lastFrameVelocity = rb.velocity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 

        boundingCollider = gameObject.AddComponent<BoxCollider>();
        boundingCollider.size = new Vector3(characterRadius, characterHeight, characterRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("LS collision");

        List<ContactPoint> contacts = new List<ContactPoint>();
        int num = collision.GetContacts(contacts);

        Vector3 avgPoint = Vector3.zero;
        Vector3 avgNormal = Vector3.zero;

        foreach(ContactPoint contact in contacts)
        {
            print("LS draw point: " + contact.point);

            avgPoint += contact.point;
            avgNormal += contact.normal;    
        }

        avgNormal /= num;
        avgPoint /= num;

        Debug.DrawLine(avgPoint, avgPoint + avgNormal * 500, Color.blue, 100.0f);

        float dot = Vector3.Dot(avgNormal, Vector3.up);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        print("LS angle: " + angle);

        if(angle > minSlopeGradation) 
        {
            if (!isInterpolating)
            {
                initialVelocity = lastFrameVelocity;
                print("LS set init vel: " + initialVelocity);
            }
            SnapUp(); 
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
            rb.velocity = Vector3.zero;
            StartCoroutine(InterpCoroutine(hit.point + transform.up * characterHeight / 2));
        }
    }


    private IEnumerator InterpCoroutine(Vector3 targetPos)
    { 
        isInterpolating = true;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();

        while(Vector3.Distance(transform.position, targetPos) > .5f)
        {
            rb.velocity = Vector3.zero;

            Vector3 point1 = transform.position - (transform.up * (capsule.height / 2 - capsule.radius));
            Vector3 point2 = point1 + (transform.up * (capsule.height - capsule.radius));
            Vector3 delta = Vector3.Slerp(transform.position, targetPos, Time.fixedDeltaTime * interpSpeed) - transform.position;
            RaycastHit hit;

            Debug.DrawLine(point1, point2, Color.magenta, 100);
            Debug.DrawLine(point1 + delta, point2 + delta, Color.magenta, 100);

            if (Physics.CapsuleCast(point1, point2, capsule.radius, delta.normalized, out hit, delta.magnitude))
            {
                print("LS capsule hit");

                delta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * delta.magnitude;
            }

            transform.position += delta;

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = initialVelocity;
        isInterpolating = false;

        print("LS interp end. Velocity: " + initialVelocity);
    }

    public bool GetIsInterpolating()
    {
        return isInterpolating;
    }
}
