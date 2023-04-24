using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{

    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;
    public UnityEvent onResurrect;

    [SerializeField] float maxHealth = 10f;
    [SerializeField] float currentHealth;
    public bool destroyOnDeath = true;
    HeadHealthBar healthBar;
    [HideInInspector] public bool isAlive = true;
    
    [SerializeField] bool testTriggerResurrect = false;

    void Awake() {
        healthBar = GetComponentInChildren<HeadHealthBar>();
        currentHealth = maxHealth;

        onDeath.AddListener(() => {
            if (destroyOnDeath) Destroy(gameObject);
        });

        healthBar.Set(currentHealth, maxHealth);
    }

    void FixedUpdate() {
        if (testTriggerResurrect)
        {
            testTriggerResurrect = false;
            Resurrect();
        }
    }

    public virtual float GetHealth() {
        return currentHealth;
    }

    public virtual void SetHealth(float hp) {
        if (hp > maxHealth) currentHealth = maxHealth;
        else currentHealth = hp;

        healthBar.Set(currentHealth, maxHealth);
    }

    public virtual float GetMaxHealth() {
        return maxHealth;
    }

    public virtual void SetMaxHealth(float hp) {
        maxHealth = hp;

        healthBar.Set(currentHealth, maxHealth);
    }

    public virtual void HealPercent(float percent) {
        float hp = maxHealth * percent / 100f;
        SetHealth(hp);
    }

    public virtual void HealFlat(float amount) {
        SetHealth(currentHealth + amount);
    }

    public virtual void TakeDamage(float hp) {
        currentHealth -= hp;
        if (currentHealth < 0f) currentHealth = 0f;
        healthBar.Set(currentHealth, maxHealth);

        onDamageTaken.Invoke();

        Debug.LogWarning("Got hurt: " + gameObject.name);

        if (currentHealth == 0f) {
            Die();
        }
    }

    public virtual void Die() {
        currentHealth = 0f;
        isAlive = false;

        onDeath.Invoke();

        Debug.LogWarning("Died: " + gameObject.name);
    }

    public virtual void Resurrect() {
        currentHealth = 1f;
        isAlive = true;

        onResurrect.Invoke();

        Debug.LogWarning("Resurrected: " + gameObject.name);
    }

}
