using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayManager : MonoBehaviour
{
    private float MaxHealth = 100;
    private float CurrentHealth = 100;

    public Slider healthBar;

    public bool resetOnStart = false;

    private void Start()
    {
        if(resetOnStart)
        {
            Reset();
        }
    }

    public HealthDisplayManager() 
    {
        CurrentHealth = MaxHealth;
    }

    public void SetMaxHealth(float inMaxHealth)
    {
        MaxHealth = inMaxHealth;
        UpdateHealthBar();
    }

    public void SetCurrentHealth(float inHealth)
    {
        CurrentHealth = inHealth;
        UpdateHealthBar();
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.value = CurrentHealth / MaxHealth;
        }
    }
}
