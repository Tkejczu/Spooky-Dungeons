using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public GameObject[] drops = new GameObject[17];
    bool isDead = false;
    public override void Die()
    {
        base.Die();
        Vector3 dropPosition = transform.position;
        
        if (drops != null && !isDead)
        {
            int randomIndex = Random.Range(0, drops.Length);
            Instantiate(drops[randomIndex], dropPosition, Quaternion.Euler(new Vector3(90, 0, 0)));
            isDead = true;
        }                
        Destroy(gameObject);
    }
}
