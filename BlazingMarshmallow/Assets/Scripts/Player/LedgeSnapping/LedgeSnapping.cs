using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LedgeSnapping : MonoBehaviour
{

    protected Rigidbody rb;

    [SerializeField] protected float minSlopeGradation = 88;
    [SerializeField] protected float maxSnapHeight = 10;
    [SerializeField] protected float forwardProbeDistance = 1;
    [SerializeField] protected float interpSpeed = 1;
    [SerializeField] protected float characterHeight = 2;

    protected Vector3 initialVelocity;
    protected bool isInterpolating = false;

    private bool hasCollided;

    private void Update()
    {
        //initialVelocity = rb.velocity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        initialVelocity = -collision.relativeVelocity;

        print("LS collision");

        //if(hasCollided) { return; }
        //if(collision.collider.gameObject.layer != LayerMask.NameToLayer("Claw")) { return; }

        hasCollided = true;

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

        if(angle > minSlopeGradation ) { SnapUp(); }


    }

    private void SnapUp()
    {
        Vector3 end = transform.position + (transform.forward * forwardProbeDistance) - (transform.up * characterHeight / 2);
        Vector3 start = end + transform.up * maxSnapHeight;
        //Vector3 end = start - transform.up * (maxSnapHeight + characterHeight / 2);
        RaycastHit hit;

        Debug.DrawLine(start, end, Color.green, 100);

        if(!isInterpolating && Physics.Linecast(start, end, out hit))
        {
            //transform.position = hit.point + transform.up * 2f;
            rb.velocity = Vector3.zero;
            StartCoroutine(InterpCoroutine(hit.point + transform.up));
        }
    }


    private IEnumerator InterpCoroutine(Vector3 targetPos)
    {
        
        isInterpolating = true;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();

        while(Vector3.Distance(transform.position, targetPos) > 1f)
        {
            rb.velocity = Vector3.zero;

            Vector3 point1 = transform.position - (transform.up * (capsule.height / 2 - capsule.radius));
            Vector3 point2 = point1 + (transform.up * (capsule.height - capsule.radius));
            Vector3 delta = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * interpSpeed) - transform.position;
            RaycastHit hit;

            Debug.DrawLine(point1, point2, Color.magenta, 100);
            Debug.DrawLine(point1 + delta, point2 + delta, Color.magenta, 100);

            if (Physics.CapsuleCast(point1, point2, capsule.radius, delta.normalized, out hit, delta.magnitude))
            {
                print("LS capsule hit");
                /*
                Vector3 direction;
                float dist;
                bool b = Physics.ComputePenetration(capsule, transform.position, transform.rotation, hit.collider, hit.collider.transform.position, hit.collider.transform.rotation, out direction, out dist);
                if(b)
                {

                }
                */

                delta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * delta.magnitude;
            }

            transform.position += delta;

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = initialVelocity;
        isInterpolating = false;

        print("LS interp end");

        StopAllCoroutines();
    }
}
