using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage = 1f;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      
            // Destroy bullet
            Destroy(gameObject);
       
    }
}
