using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeSnapping : MonoBehaviour
{

    protected Rigidbody rb;

    [SerializeField] protected float minSlopeGradation = 88;
    [SerializeField] protected float maxSnapHeight = 10;
    [SerializeField] protected float forwardProbeDistance = 1;

    protected Vector3 initialVelocity;

    private bool hasCollided;

    private void Update()
    {
        initialVelocity = rb.velocity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
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

        if(angle > minSlopeGradation) { SnapUp(); }


    }

    private void SnapUp()
    {
        Vector3 end = transform.position + transform.forward * forwardProbeDistance;
        Vector3 start = end + Vector3.up * maxSnapHeight;
        RaycastHit hit;

        Debug.DrawLine(start, end, Color.green, 100);

        if(Physics.Linecast(start, end, out hit))
        {
            transform.position = hit.point + transform.up * 2f;

            rb.velocity = initialVelocity;
        }
    }
}
