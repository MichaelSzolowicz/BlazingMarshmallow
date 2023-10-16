using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Michael Szolowicz]
/// Controls player status and properties.
/// </summary>

public class PlayerStats_Szolo : MonoBehaviour
{
    public enum Status { Nuetral, Burned };

    public Status currentStatus = Status.Nuetral;
    [Tooltip("Frequency at which burn status effects are updated.")]
    public float burnRate = 0.5f;
    [Tooltip("Amount of health lost every time burn status effects update.")]
    public float burnDamage = 1.0f;
    public float health = 100.0f;
    public int Collectables = 0;

    // Stat change delegates
    public delegate void InflictBurnDelegate();
    InflictBurnDelegate onInflictBurn;
    public delegate void ClearBurnDelegate();
    ClearBurnDelegate onClearBurn;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Fire")
        {
            InflictBurn();
        }
        if (other.gameObject.tag == "Chocolate")
        {
            Collectables++;
            //set the other game object to false
            other.gameObject.SetActive(false);           
        }
    }

    
    /// <summary>
    /// Update current status to burned, start burn coroutine.
    /// </summary>
    public void InflictBurn()
    {
        currentStatus = Status.Burned;

        if(onInflictBurn.GetInvocationList().Length > 0 ) onInflictBurn.Invoke();

        StartCoroutine(Burn());
    }

    /// <summary>
    /// Subtract life while player is burned.
    /// Called at frequency specified by burnRate.
    /// </summary>
    protected IEnumerator Burn()
    {
        while(currentStatus == Status.Burned)
        {
            health -= burnDamage;
            yield return new WaitForSeconds(burnRate);
        }

        if(onClearBurn.GetInvocationList().Length > 0 ) { onClearBurn.Invoke(); }  
        
        StopCoroutine(Burn());
    }

    /// <summary>
    /// Add an external event to be called when burn is inflicted.
    /// </summary>
    /// <param name="callback"></param>
    public void AddInflictBurnCallback(InflictBurnDelegate callback)
    {
        onInflictBurn += callback;
    }

    /// <summary>
    /// Add and external event to be called as soon as status is no longer burning.
    /// </summary>
    /// <param name="callback"></param>
    public void AddClearBurnCallback(ClearBurnDelegate callback)
    {
        onClearBurn += callback;
    }
}
