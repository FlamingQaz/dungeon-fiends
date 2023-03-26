using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{

    public UnityEvent onHurt;
    public UnityEvent onDeath;

    public const float maxHealth = 10f;
    public float health = maxHealth;

    public virtual void Hurt(float hp) {
        health -= hp;
        onHurt.Invoke();

        Debug.LogWarning("Got hurt: " + gameObject.name);

        if (health <= 0f) {
            health = 0f;
            Die();
        }
    }

    public virtual void Die() {
        health = 0f;
        onDeath.Invoke();

        Debug.LogWarning("Died: " + gameObject.name);
    }

}
