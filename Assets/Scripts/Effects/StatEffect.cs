using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : Effect
{
    public enum ConditionComparison {
        [InspectorName("=")] Equals,
        [InspectorName(">")] Greater,
        [InspectorName("<")] Less,
        [InspectorName(">=")] GreaterOrEqual,
        [InspectorName("<=")] LessOrEqual,
    }
    
    public enum ConditionStat {
        HealthMax,
        HealthCurrent,
        Damage,
        ShieldCurrent,
        SpeedMovement,
        SpeedAttack,
        ResistanceCombat,
        ResistanceEffect,
        ResistanceEnvironment
    }

    [System.Serializable]
    public struct Condition {
        public ConditionStat type;
        public ConditionComparison when;
        public float value;
    }

    [Header("Stat - General Stat Buffs/Debuffs For a Length of Time")]
    [Tooltip("A list of conditions which, if true, will trigger stat changes.")]
    public List<Condition> conditions;
    [Tooltip("Changes to stats. Positives increase the stat, negatives decrease the stat, and 0 does not change the stat.")]
    public Entity.Stats statChanges = Entity.Stats.WithDefaultAs(0f);

    bool hasModified = false;

    protected override void Start()
    {
        base.Start();
        
        OnProc(ModifyStats);
        OnEnd(() => {
            if (hasModified) target.RemoveRawStats(statChanges);
        });

        BeginEffect();
    }

    void ModifyStats() {
        bool shouldModify = true;

        foreach (Condition condition in conditions) {
            float required = condition.value;
            float actual = 0f;

            Entity.Stats stats = target.GetRawStats();

            switch (condition.type) {
                case ConditionStat.HealthMax:
                    actual = stats.maxHealth;
                break;
                case ConditionStat.HealthCurrent:
                    actual = stats.currentHealth;
                break;
                case ConditionStat.ShieldCurrent:
                    actual = stats.currentShield;
                break;
                case ConditionStat.Damage:
                    actual = stats.damage;
                break;
                case ConditionStat.SpeedMovement:
                    actual = stats.movementSpeed;
                break;
                case ConditionStat.SpeedAttack:
                    actual = stats.attackSpeed;
                break;
                case ConditionStat.ResistanceCombat:
                    actual = stats.resistance.Combat;
                break;
                case ConditionStat.ResistanceEffect:
                    actual = stats.resistance.Effect;
                break;
                case ConditionStat.ResistanceEnvironment:
                    actual = stats.resistance.Environment;
                break;
            }

            if ((condition.when == ConditionComparison.Greater && actual <= required) ||
                (condition.when == ConditionComparison.Less && actual >= required) || 
                (condition.when == ConditionComparison.GreaterOrEqual && actual < required) || 
                (condition.when == ConditionComparison.LessOrEqual && actual > required) ||
                (condition.when == ConditionComparison.Equals && actual != required)) shouldModify = false;
        }

        if (shouldModify && !hasModified) {
            target.AddRawStats(statChanges);
            hasModified = true;
        }
        else if (!shouldModify && hasModified) {
            target.RemoveRawStats(statChanges);
            hasModified = false;
        }
    }
}
