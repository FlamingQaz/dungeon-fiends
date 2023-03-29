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
    public struct Modifier {
        public Effect effect;
        public EffectTrigger trigger;
        public EffectTarget target;
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

    void ApplyToSelfOnly(UnityEvent e, Modifier modifier) {
        Effect effect = modifier.effect;
        EffectTrigger trigger = modifier.trigger;
        EffectTarget targetType = modifier.target;

        if (targetType != EffectTarget.Self) {
            Debug.LogError($"Invalid target for {trigger.ToString()} Entity Modifier.");
            return;
        }
        
        if (e != null) e.AddListener(() => effect.ApplyTo(gameObject));
        else effect.ApplyTo(gameObject);
    }

    void ApplyToAnyEntity(EntityEvent e, Modifier modifier) {
        Effect effect = modifier.effect;
        EffectTarget targetType = modifier.target;

        e.AddListener((Entity target) => {
            if (targetType == EffectTarget.TargetedOpponent) effect.ApplyTo(target.gameObject, gameObject);
            else if (targetType == EffectTarget.Self) effect.ApplyTo(gameObject, target.gameObject);
        });
    }
}
