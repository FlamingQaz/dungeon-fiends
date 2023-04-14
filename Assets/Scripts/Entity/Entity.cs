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
    public UnityEvent onHeal = new UnityEvent();

    [SerializeField] float maxHealth = 10f;
    public bool destroyOnDeath = true;
    float currentHealth;
    HeadHealthBar healthBar;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool onKillTriggered = false;
    public bool targetable = true;
    [SerializeField] bool testTriggerResurrect = false;
    [SerializeField] bool debugMessages = false;

    public enum DamageType {
        /// <summary>(Default) Basic damage from weapons, attacks, and miscellaneous damage sources.</summary>
        Combat,
        /// <summary>Damage from the environment, such as spikes and traps.</summary>
        Environment,
        /// <summary>Damage from effects, such as fire or poison.</summary>
        Effect
    }

    [System.Serializable]
    public struct Resistances {
        [Tooltip("(Default) Basic damage from weapons, attacks, and miscellaneous damage sources.")]
        public float Combat;
        [Tooltip("Damage from the environment, such as spikes and traps.")]
        public float Environment;
        [Tooltip("Damage from effects, such as fire or poison.")]
        public float Effect;

        public float Get(DamageType type) {
            switch (type) {
                case DamageType.Combat:
                    return Combat;
                case DamageType.Environment:
                    return Environment;
                case DamageType.Effect:
                    return Effect;
                default:
                    return 0f;
            }
        }
    }

    public Resistances resistance;

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
        SetHealth(currentHealth + hp);
        onHeal.Invoke();
    }

    public virtual void HealFlat(float amount) {
        SetHealth(currentHealth + amount);
        onHeal.Invoke();
    }

    public virtual void TakeDamage(float hp, DamageType type = DamageType.Combat) {
        // Handle resistances
        hp -= resistance.Get(type);
        if (hp <= 0f) return;

        // Handle health reduction
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
