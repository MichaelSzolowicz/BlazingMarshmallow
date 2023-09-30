using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Kriger, Nathan
//09/26/2023

public class ballController : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 moveDir = Vector3.up + Vector3.left;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    private void move()
    {
        transform.Translate(moveDir * Time.deltaTime * speed);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Hit Wall");
            //multiply by -1 to reverse direction of ball
            //can choose which axis specifically or to do all of them
            //moveDir = new Vector3(moveDir.x, moveDir.y * -1);
            moveDir.y *= -1;
        }
        if (collision.gameObject.tag == "SideWall")
        {
            Debug.Log("Hit Side Wall");
            //moveDir = new Vector3(moveDir.x * -1, moveDir.y);
            moveDir.x *= -1;
        }
    }
}
