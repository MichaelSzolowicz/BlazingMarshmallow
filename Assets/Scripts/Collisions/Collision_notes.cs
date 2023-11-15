using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_notes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*Types of Collisions
     
      OnCollisionEnter
        - When two objects collide
        - When a rigidbody enters a collision

      OnCollisionStay
        - When two objects are colliding constantly

      OnCollisionExit
        - When two objects stop colliding


    Collisions and Tags

    if (collision.info.tag == "Player")
    {
        // Do something
    }

    Physics.IgnoreCollision
        - Ignores collisions between two objects
        Physica.IgnoreCollision(gameObject.collider, anotherObject.collider);

    Layer Collision Matrix
        - Edit -> Project Settings -> Physics
        - Edit the matrix to change which layers collide with each other
    */

}
