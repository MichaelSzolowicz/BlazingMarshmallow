using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public AudioSource checkpointSound;
    public bool checkpointClear = false;

    private void OnTriggerEnter(Collider other)
    {
        checkpointSound.Play();
    }
}
