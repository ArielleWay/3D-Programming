using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the interface outside of any class
public interface IDamageable
{
    void TakeDamage(float damage);
    void Die();
    float GetHealth();
}

// Create a class that implements the IDamageable interface
public class Damageable : MonoBehaviour, IDamageable
{
    // Implement the methods from the IDamageable interface
    public void TakeDamage(float damage)
    {
        // Logic to handle taking damage
    }

    public void Die()
    {
        // Logic to handle death
    }

    public float GetHealth()
    {
        // Logic to return the current health
        return 0f; // Example return value
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
