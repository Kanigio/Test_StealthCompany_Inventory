using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO_HealthComponent_FirstPerson : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (IsDead)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
