using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModifier : MonoBehaviour
{

    public enum EffectTrigger {
        OnDamageTaken,
        OnDeath,
        OnResurrect,
        OnAttack,
        OnKill,
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
            Effect effect = modifier.effect;
            EffectTrigger trigger = modifier.trigger;
            EffectTarget targetType = modifier.target;

            switch (trigger) {
                case EffectTrigger.OnDamageTaken:
                    if (targetType != EffectTarget.Self) {
                        Debug.LogError("Invalid target for OnDamageTaken Entity Modifier.");
                        return;
                    }
                    entity.onDamageTaken.AddListener(() => effect.ApplyTo(gameObject));
                break;
                case EffectTrigger.OnDeath:
                    if (targetType != EffectTarget.Self) {
                        Debug.LogError("Invalid target for OnDeath Entity Modifier.");
                        return;
                    }
                    entity.onDeath.AddListener(() => effect.ApplyTo(gameObject));
                break;
                case EffectTrigger.OnResurrect:
                    if (targetType != EffectTarget.Self) {
                        Debug.LogError("Invalid target for OnResurrect Entity Modifier.");
                        return;
                    }
                    entity.onResurrect.AddListener(() => effect.ApplyTo(gameObject));
                break;
                case EffectTrigger.OnAttack:
                    entity.onEndAttack.AddListener((Entity target) => {
                        if (targetType == EffectTarget.TargetedOpponent) effect.ApplyTo(target.gameObject, gameObject);
                        else if (targetType == EffectTarget.Self) effect.ApplyTo(gameObject, target.gameObject);
                    });
                break;
                case EffectTrigger.OnKill:
                    entity.onKill.AddListener((Entity target) => {
                        if (targetType == EffectTarget.TargetedOpponent) effect.ApplyTo(target.gameObject, gameObject);
                        else if (targetType == EffectTarget.Self) effect.ApplyTo(gameObject, target.gameObject);
                    });
                break;
                case EffectTrigger.Infused:
                    if (targetType != EffectTarget.Self) {
                        Debug.LogError("Invalid target for OnResurrect Entity Modifier.");
                        return;
                    }
                    effect.ApplyTo(gameObject);
                break;
            }
        }
    }
}
