using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GO_HealthComponent_FirstPerson : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    public event Action OnHealthChanged;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (IsDead)
        {
            Die();
        }
        
        Debug.LogWarning("GameObject Took Damage!");
        OnHealthChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
    
    public float CurrentHealth => currentHealth;
}
