using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    private List<IDamageable> enemiesInDamageZone = new List<IDamageable>();

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !enemiesInDamageZone.Contains(damageable))
        {
            enemiesInDamageZone.Add(damageable);
            Debug.Log("Enemy entered damage zone. " + other.gameObject.name);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && enemiesInDamageZone.Contains(damageable))
        {
            enemiesInDamageZone.Remove(damageable);
            Debug.Log("Enemy left damage zone " + other.gameObject.name);
        }

    }

    public void GetClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        //float closestEnemy;
        foreach (Transform enemy in enemiesInDamageZone)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    //closestEnemy = enemy.GetComponent<IDamageable>();
                }
            }
        }
    }

    public void DamageEnemy()
    {
        //closestEnemy.TakeDamage(5);
    }

    public void DealDamageToEnemies()
    {
        foreach (var enemy in enemiesInDamageZone)
        {
            enemy.TakeDamage(5);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnDrawGizmos()
    //{
    //    if (sphereCollider != null)
    //    {
    //        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
    //        Gizmos.DrawSphere(sphereCollider.transform.position, sphereCollider.radius);
    //    }

    //    Gizmos.color = Color.red;
    //    foreach (var enemy in enemiesInRange)
    //    {
    //        if (DamageEnemy != null)
    //        {
    //            Gizmos.DrawSphere(enemy.transform.position, 0.05f);
    //        }
    //    }
    //}


}
