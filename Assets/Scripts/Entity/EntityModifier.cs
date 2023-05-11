using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityModifier : MonoBehaviour
{

    public enum EffectTrigger {
        OnDamageTaken,
        OnDeath,
        OnResurrect,
        OnAttack,
        OnKill,
        OnHeal,
        Infused
    }

    public enum EffectTarget {
        Self,
        TargetedOpponent
    }

    [System.Serializable]
    public class Modifier {
        public Effect effect;
        [Tooltip("The event that causes the effect to be applied. Choosing the infused event immediately applies the effect when the desired difficulty level is reached.")]
        public EffectTrigger trigger;
        public EffectTarget target;
        [Tooltip("The effect can only be applied if the difficulty level is greater than or equal to this level.")]
        public DifficultyScaling.Level enableAtDifficulty = DifficultyScaling.Level.Any;
    }

    public List<Modifier> modifiers;
    Entity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();

        foreach (Modifier modifier in modifiers) {
            switch (modifier.trigger) {
                case EffectTrigger.OnDamageTaken:
                    ApplyToSelfOnly(entity.onDamageTaken, modifier);
                break;
                case EffectTrigger.OnDeath:
                    ApplyToSelfOnly(entity.onDeath, modifier);
                break;
                case EffectTrigger.OnResurrect:
                    ApplyToSelfOnly(entity.onResurrect, modifier);
                break;
                case EffectTrigger.OnAttack:
                    ApplyToAnyEntity(entity.onEndAttack, modifier);
                break;
                case EffectTrigger.OnKill:
                    ApplyToAnyEntity(entity.onKill, modifier);
                break;
                case EffectTrigger.OnHeal:
                    ApplyToSelfOnly(entity.onHeal, modifier);
                break;
                case EffectTrigger.Infused:
                    ApplyToSelfOnly(null, modifier);
                break;
            }
        }
    }

    IEnumerator ApplyToSelfDelayed(Effect effect) {
        yield return new WaitForSeconds(0.1f);
        effect.ApplyTo(gameObject);
    }

    IEnumerator ApplyToEntityDelayed(Effect effect, GameObject target, GameObject other) {
        yield return new WaitForSeconds(0.1f);
        effect.ApplyTo(target, other);
    }

    void ApplyToSelfOnly(UnityEvent e, Modifier modifier) {
        Effect effect = modifier.effect;
        EffectTrigger trigger = modifier.trigger;
        EffectTarget targetType = modifier.target;
        DifficultyScaling.Level difficulty = modifier.enableAtDifficulty;

        if (targetType != EffectTarget.Self) {
            Debug.LogError($"Invalid target for {trigger.ToString()} Entity Modifier.");
            return;
        }

        if (e == null && (difficulty == DifficultyScaling.Level.Any || DifficultyScaling.GetLevel() >= difficulty)) StartCoroutine(ApplyToSelfDelayed(effect));
        else if (e != null) e.AddListener(() => {
            if (difficulty == DifficultyScaling.Level.Any || DifficultyScaling.GetLevel() >= difficulty) StartCoroutine(ApplyToSelfDelayed(effect));
        });
        
        // Handle adding and removing infused effects at various difficulties:
        bool currentlyApplied = false;
        DifficultyScaling.onChange.AddListener(() => {
            if (difficulty == DifficultyScaling.Level.Any || e != null) return;

            if (DifficultyScaling.GetLevel() < difficulty && currentlyApplied) {
                effect.GetExecutableEffect(entity).EndEffect();
                currentlyApplied = false;
            }
            else if (DifficultyScaling.GetLevel() >= difficulty && !currentlyApplied) {
                StartCoroutine(ApplyToSelfDelayed(effect));
                currentlyApplied = true;
            }
        });
    }

    void ApplyToAnyEntity(EntityEvent e, Modifier modifier) {
        Effect effect = modifier.effect;
        EffectTarget targetType = modifier.target;
        DifficultyScaling.Level difficulty = modifier.enableAtDifficulty;

        e.AddListener((Entity target) => {
            bool canApply = difficulty == DifficultyScaling.Level.Any || DifficultyScaling.GetLevel() >= difficulty;
            if (!canApply) return;

            if (targetType == EffectTarget.TargetedOpponent) StartCoroutine(ApplyToEntityDelayed(effect, target.gameObject, gameObject));
            else if (targetType == EffectTarget.Self) StartCoroutine(ApplyToEntityDelayed(effect, gameObject, target.gameObject));
        });
    }
}
