using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EntityModifier))]
public class Entity : MonoBehaviour
{
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

    [System.Serializable]
    public class Stats {
        public float maxHealth = 10f;
        [ReadOnly] public float currentHealth = 10f;
        public float currentShield = 0f;
        public float damage = 2f;
        public float movementSpeed = 3f;
        public float attackSpeed = 0.5f;
        public Resistances resistance;

        public static Stats WithDefaultAs(float def) {
            Stats s = new Stats();
            s.maxHealth = def;
            s.currentHealth = def;
            s.damage = def;
            s.movementSpeed = def;
            s.attackSpeed = def;
            s.resistance.Combat = def;
            s.resistance.Effect = def;
            s.resistance.Environment = def;

            return s;
        }
    }

    [Header("Options")]
    [SerializeField] Stats stats;
    public bool destroyOnDeath = true;
    public bool targetable = true;

    [Header("Debugging")]
    [SerializeField] bool testTriggerResurrect = false;
    [SerializeField] bool debugMessages = false;

    [Header("Events")]
    public UnityEvent onDamageTaken = new UnityEvent();
    public UnityEvent onDeath = new UnityEvent();
    public UnityEvent onResurrect = new UnityEvent();
    public EntityEvent onKill = new EntityEvent();
    public EntityEvent onStartAttack = new EntityEvent();
    public EntityEvent onEndAttack = new EntityEvent();
    public UnityEvent onHealthChange = new UnityEvent();
    public UnityEvent onHeal = new UnityEvent();

    // Hidden
    HeadHealthBar healthBar;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool onKillTriggered = false;

    void Awake() {
        healthBar = GetComponentInChildren<HeadHealthBar>();
        stats.currentHealth = stats.maxHealth;

        onDeath.AddListener(() => {
            foreach (Effect effect in CurrentEffects()) {
                effect.EndEffect();
            }

            if (destroyOnDeath) {
                Destroy(gameObject);
            }
        });

        healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
    }

    void FixedUpdate() {
        if (testTriggerResurrect)
        {
            testTriggerResurrect = false;
            Resurrect();
        }
    }

    public virtual float GetHealth() {
        return stats.currentHealth;
    }

    public virtual void SetHealth(float hp) {
        if (hp > stats.maxHealth) stats.currentHealth = stats.maxHealth;
        else stats.currentHealth = hp;

        healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
        onHealthChange.Invoke();
    }

    public virtual float GetMaxHealth() {
        return stats.maxHealth;
    }

    public virtual void SetMaxHealth(float hp) {
        stats.maxHealth = hp;

        healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
        onHealthChange.Invoke();
    }

    public virtual float GetShield() {
        return stats.currentShield;
    }

    public virtual void SetShield(float hp) {
        stats.currentShield = hp;
        healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
    }

    public virtual void HealPercent(float percent) {
        float hp = stats.maxHealth * percent / 100f;
        SetHealth(stats.currentHealth + hp);
        onHeal.Invoke();
    }

    public virtual void HealFlat(float amount) {
        SetHealth(stats.currentHealth + amount);
        onHeal.Invoke();
    }

    public virtual float GetAttackDamage() {
        return stats.damage;
    }

    public virtual void SetAttackDamage(float amount) {
        stats.damage = amount;
    }

    public virtual float GetMoveSpeed() {
        return stats.movementSpeed;
    }

    public virtual void SetMoveSpeed(float amount) {
        stats.movementSpeed = amount;
    }

    public virtual float GetAttackSpeed() {
        return stats.attackSpeed;
    }

    public virtual void SetAttackSpeed(float amount) {
        stats.attackSpeed = amount;
    }

    public virtual Stats GetRawStats() {
        return stats;
    }

    public virtual void SetRawStats(Stats entityStats) {
        if (entityStats.attackSpeed > -1f) stats.attackSpeed = entityStats.attackSpeed;
        if (entityStats.movementSpeed > -1f) stats.movementSpeed = entityStats.movementSpeed;
        if (entityStats.damage > -1f) stats.damage = entityStats.damage;
        if (entityStats.maxHealth > -1f) {
            stats.maxHealth = entityStats.maxHealth;
            healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
            onHealthChange.Invoke();
        }
        if (entityStats.resistance.Combat > -1f) stats.resistance.Combat = entityStats.resistance.Combat;
        if (entityStats.resistance.Effect > -1f) stats.resistance.Effect = entityStats.resistance.Effect;
        if (entityStats.resistance.Environment > -1f) stats.resistance.Environment = entityStats.resistance.Environment;
    }

    public virtual void AddRawStats(Stats entityStats) {
        stats.attackSpeed += entityStats.attackSpeed;
        stats.movementSpeed += entityStats.movementSpeed;
        stats.damage += entityStats.damage;
        stats.maxHealth += entityStats.maxHealth;
        stats.resistance.Combat += entityStats.resistance.Combat;
        stats.resistance.Effect += entityStats.resistance.Effect;
        stats.resistance.Environment += entityStats.resistance.Environment;

        if (entityStats.maxHealth != 0f) {
            healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
            onHealthChange.Invoke();
        }
    }

    public virtual void RemoveRawStats(Stats entityStats) {
        stats.attackSpeed -= entityStats.attackSpeed;
        stats.movementSpeed -= entityStats.movementSpeed;
        stats.damage -= entityStats.damage;
        stats.maxHealth -= entityStats.maxHealth;
        stats.resistance.Combat -= entityStats.resistance.Combat;
        stats.resistance.Effect -= entityStats.resistance.Effect;
        stats.resistance.Environment -= entityStats.resistance.Environment;

        if (entityStats.maxHealth != 0f) {
            healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
            onHealthChange.Invoke();
        }
    }

    public virtual void TakeDamage(float hp, DamageType type = DamageType.Combat) {
        // Handle resistances
        hp -= stats.resistance.Get(type);
        if (hp <= 0f) return;

        // Handle shield
        stats.currentShield -= hp;
        onDamageTaken.Invoke();
        if (stats.currentShield >= 0f) hp = 0f;
        else if (stats.currentShield < 0f) stats.currentShield = 0f;

        // Handle health reduction
        stats.currentHealth -= hp;
        if (stats.currentHealth < 0f) stats.currentHealth = 0f;

        healthBar.Set(stats.currentHealth, stats.maxHealth, stats.currentShield);
        if (hp > 0f) onHealthChange.Invoke();

        if (debugMessages) Debug.LogWarning("Got hurt: " + gameObject.name);

        if (stats.currentHealth == 0f) {
            Die();
        }
    }

    public virtual void Die() {
        stats.currentHealth = 0f;
        isAlive = false;

        onDeath.Invoke();

        if (debugMessages) Debug.LogWarning("Died: " + gameObject.name);
    }

    public virtual void Resurrect() {
        stats.currentHealth = 1f;
        isAlive = true;
        onKillTriggered = false;

        onResurrect.Invoke();

        if (debugMessages) Debug.LogWarning("Resurrected: " + gameObject.name);
    }

    public virtual Effect[] CurrentEffects() {
        return GetComponentsInChildren<Effect>();
    }

}
