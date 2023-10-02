using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player status and properties.
// Michael Szolowicz
// 10/1/2023

public class PlayerStats_Szolo : MonoBehaviour
{
    public enum Status { Nuetral, Burned };

    public Status currentStatus = Status.Nuetral;
    [Tooltip("Frequency at which burn status effects are updated.")]
    public float burnRate = 0.5f;
    [Tooltip("Amount of health lost every time burn status effects update.")]
    public float burnDamage = 1.0f;

    public float health = 100.0f;

    private float testTimer = 0.0f;

    private void Update()
    {
        //TESTONLY
        testTimer += Time.deltaTime;

        if(testTimer > 5.0f && currentStatus != Status.Burned)
        {
            //InflictBurn();
        }
        //ENDTEST
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Fire")
        {
            InflictBurn();
        }
    }

    /// <summary>
    /// Update current status to burned, start burn coroutine.
    /// </summary>
    public void InflictBurn()
    {
        currentStatus = Status.Burned;
        StartCoroutine(Burn());
    }

    /// <summary>
    /// Subtract life while player is burned.
    /// Called at frequency specified by burnRate.
    /// </summary>
    protected IEnumerator Burn()
    {
        while(true)
        {
            health -= burnDamage;
            yield return new WaitForSeconds(burnRate);
        }
    }
}
