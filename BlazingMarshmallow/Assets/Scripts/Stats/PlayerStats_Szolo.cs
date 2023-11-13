//using OpenCover.Framework.Model;
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

    [Tooltip("Public only for testing purposes.")]
    public Status currentStatus = Status.Nuetral;
    [Tooltip("Frequency at which burn status effects are updated.")]
    public float burnRate = 0.5f;
    [Tooltip("Amount of health lost every time burn status effects update.")]
    public float burnDamage = 1.0f;
    [Tooltip("Max health.")]
    public float health = 100.0f;
    [Tooltip("Set to health on start. Public only for testing purposes.")]
    public float currentHealth;
    public int Collectables = 0;
    public int chocoBites = 0;
    public float pokeDamage;


    private HealthDisplayManager healthDisplay;

    // Stat change delegates
    public delegate void InflictBurnDelegate();
    InflictBurnDelegate onInflictBurn;
    public delegate void ClearBurnDelegate();
    ClearBurnDelegate onClearBurn;
    public GameObject Victory;

    private void Start()
    {
        currentStatus = Status.Nuetral;
        currentHealth = health;

        healthDisplay = (HealthDisplayManager)FindFirstObjectByType(typeof(HealthDisplayManager));
        if(healthDisplay)
        {
            healthDisplay.SetMaxHealth(health);
            healthDisplay.Reset();
        }

        LevelTransitions levels = FindObjectOfType<LevelTransitions>();
        if(levels != null && levels.spawnPoint == Vector3.zero)
        {
            levels.spawnPoint = transform.position;
        }
    }

    private void FixedUpdate()
    {
        hpDeath();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "pokeball")
        {
            //subtrack the poke damage f the player is hit by a pokeball
            TakeDamage(pokeDamage);
            print("player hit by pokeball");

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Fire")
        {
            InflictBurn();
        }
        else if (other.gameObject.tag == "Water")
        {
            if (onClearBurn.GetInvocationList().Length > 0) { onClearBurn.Invoke(); }
            Status prevStatus = currentStatus;
            ResetStatus();
            ResetHealth();
            if (prevStatus == Status.Burned)
            {
                print("water cleared burn");
                
            }
        }

        
        if (other.gameObject.tag == "Chocolate")
        {
            Collectables++;
            //set the other game object to false
            other.gameObject.SetActive(false);           
        }

        if (other.gameObject.tag == "ChocoBite")
        {
            chocoBites++;
            //set the other game object to false
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Victory")
        {
            print("You win!");
            Victory.SetActive(true);
            
        }

        if (other.gameObject.tag == "Checkpoint")
        {
            LevelTransitions levels = FindObjectOfType<LevelTransitions>();
            if (levels != null)
            {
                levels.spawnPoint = other.transform.root.position;
            }
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
            TakeDamage(burnDamage);
            yield return new WaitForSeconds(burnRate);
        }

        if(onClearBurn.GetInvocationList().Length > 0 ) { onClearBurn.Invoke(); }  
        
        StopCoroutine(Burn());
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(healthDisplay) healthDisplay.SetCurrentHealth(currentHealth);
    }

    /// <summary>
    /// Set current status to nuetral.
    /// </summary>
    public void ResetStatus()
    {
        currentStatus = Status.Nuetral;
    }


    /// <summary>
    /// Set current health to default health.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = health;
        if (healthDisplay) healthDisplay.Reset();
    }

    /// <summary>
    /// Set Collectables to 0.
    /// </summary>
    public void ResetCollectables()
    {
        Collectables = 0;
        chocoBites = 0;
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
    public void hpDeath()
    {
        if (currentHealth <= 0)
        {
            Controller controller = GetComponent<Controller>();
            print("damage Death");
            controller.Death();
        }
    }
}
