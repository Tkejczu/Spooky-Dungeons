using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    public Stat damage;
    public Stat armor;

    public HealthBar healthBar;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        if(healthBar != null)
        healthBar.SetMaxHealth(maxHealth);
        
    }
    public void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + "takes " + damage + "damage.");
        if (currentHealth <= 0)
        {
            Die();
        }
        healthBar.SetHealth(currentHealth);
    }
    public virtual void Die()
    {
        Debug.Log(transform.name + " has died");
    }   
}
