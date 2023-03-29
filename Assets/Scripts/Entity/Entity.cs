using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{

    public UnityEvent onDamageTaken = new UnityEvent();
    public UnityEvent onDeath = new UnityEvent();
    public UnityEvent onResurrect = new UnityEvent();
    public EntityEvent onKill = new EntityEvent();
    public EntityEvent onStartAttack = new EntityEvent();
    public EntityEvent onEndAttack = new EntityEvent();
    public UnityEvent onHealthChange = new UnityEvent();

    [SerializeField] float maxHealth = 10f;
    public bool destroyOnDeath = true;
    float currentHealth;
    HeadHealthBar healthBar;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool onKillTriggered = false;
    
    [SerializeField] bool testTriggerResurrect = false;
    [SerializeField] bool debugMessages = false;

    void Awake() {
        healthBar = GetComponentInChildren<HeadHealthBar>();
        currentHealth = maxHealth;

        onDeath.AddListener(() => {
            foreach (Effect effect in CurrentEffects()) {
                effect.EndEffect();
            }

            if (destroyOnDeath) {
                Destroy(gameObject);
            }
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
        onHealthChange.Invoke();
    }

    public virtual float GetMaxHealth() {
        return maxHealth;
    }

    public virtual void SetMaxHealth(float hp) {
        maxHealth = hp;

        healthBar.Set(currentHealth, maxHealth);
        onHealthChange.Invoke();
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
        onHealthChange.Invoke();

        if (debugMessages) Debug.LogWarning("Got hurt: " + gameObject.name);

        if (currentHealth == 0f) {
            Die();
        }
    }

    public virtual void Die() {
        currentHealth = 0f;
        isAlive = false;

        onDeath.Invoke();

        if (debugMessages) Debug.LogWarning("Died: " + gameObject.name);
    }

    public virtual void Resurrect() {
        currentHealth = 1f;
        isAlive = true;
        onKillTriggered = false;

        onResurrect.Invoke();

        if (debugMessages) Debug.LogWarning("Resurrected: " + gameObject.name);
    }

    public virtual Effect[] CurrentEffects() {
        return GetComponentsInChildren<Effect>();
    }

}
