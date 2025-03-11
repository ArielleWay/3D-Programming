using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public float maxHealth;
    public float currentHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Hit");
        currentHealth -= damage;
    }

    public void Die()
    {
        animator.SetTrigger("Death");
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
