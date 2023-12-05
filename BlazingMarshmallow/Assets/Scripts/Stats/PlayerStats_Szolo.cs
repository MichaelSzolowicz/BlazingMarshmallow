//using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Character Data")]
    public float currentHealth;
    public int Collectables = 0;
    public int chocoBites = 0;
    public TextMeshProUGUI chocobiteText;

    [Header("Big Choco Sprites")]
    public Image bigChoco1;
    public Image bigChoco2;
    public Image bigChoco3;

    public float time = 0;
    public TMP_Text timeText = null;    
    public float pokeDamage;
    
    public AudioSource oofSound;
    public AudioSource burnStartSound;
    public AudioSource fireOutSound;
    public AudioSource chocolateGetSound;

    private HealthDisplayManager healthDisplay;

    // Stat change delegates
    public delegate void InflictBurnDelegate();
    InflictBurnDelegate onInflictBurn;
    public delegate void ClearBurnDelegate();
    ClearBurnDelegate onClearBurn;
    public GameObject Victory;

    private void Start()
    {
        //time = 0;
        currentStatus = Status.Nuetral;
        currentHealth = health;
	
        healthDisplay = (HealthDisplayManager)FindFirstObjectByType(typeof(HealthDisplayManager));
        if(healthDisplay)
        {
            healthDisplay.SetMaxHealth(health);
            healthDisplay.Reset();
        }

        LevelTransitions levels = FindObjectOfType<LevelTransitions>();
        if (levels != null && levels.getInstance() != null)
        {
            time = levels.getInstance().playTime;
            if (levels.getInstance().spawnPoint == Vector3.zero)
            {
                levels.getInstance().spawnPoint = transform.position;
            }
        }
    }

    private void Awake()
    {

    }

    private void Update()
    {
        time += Time.deltaTime;
        if(timeText != null )
        {
            timeText.text = time.ToString();
        }
    }

    private void FixedUpdate()
    {
        hpDeath();
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "pokeball")
        {            
             print("player hit by pokeball");
             oofSound.Play();
             TakeDamage(pokeDamage);           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Fire")
        {
	        burnStartSound.Play();
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
		        fireOutSound.Play();
                print("water cleared burn");
                
            }
        }

        
        if (other.gameObject.tag == "Chocolate")
        {
	        chocolateGetSound.Play();
            Collectables++;
            currentHealth += 20;
            SpriteFill();
            if (currentHealth > 100) { currentHealth = 100; }
            if (healthDisplay) healthDisplay.SetCurrentHealth(currentHealth);
            //set the other game object to false
            other.gameObject.SetActive(false);           
        }

        if (other.gameObject.tag == "ChocoBite")
        {
	        chocolateGetSound.Play();
            chocoBites++;
            chocobiteText.text = chocoBites.ToString();
            currentHealth += 5;
            if (currentHealth > 100) { currentHealth = 100; }
            if (healthDisplay) healthDisplay.SetCurrentHealth(currentHealth);
            //set the other game object to false
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Victory")
        {
            print("You win!");
            Victory.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        if (other.gameObject.tag == "Checkpoint")
        {
            
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            LevelTransitions levels = FindObjectOfType<LevelTransitions>();
            if (levels != null & levels.getInstance() != null)
            {
                levels.getInstance().spawnPoint = other.transform.root.position;
                if(!checkpoint.checkpointClear)
                {
                    levels.getInstance().playTime = time;
                }
            }

            if(checkpoint != null) {
                checkpoint.checkpointClear = true;
            }
        }

        if(other.gameObject.tag == "Victory")
        {
            LevelTransitions levels = LevelTransitions.instance;
            if(levels != null)
            {
                levels.playTime = time;
            }
        }

    }

    void SpriteFill()
    {
        if (Collectables == 1)
        {
            bigChoco1.fillAmount = 1;
        }
        if (Collectables == 2)
        {
            bigChoco2.fillAmount = 1;
        }
        if (Collectables == 3)
        {
            bigChoco3.fillAmount = 1;
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
