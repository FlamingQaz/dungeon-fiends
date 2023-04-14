using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechEffect : Effect
{
    [Header("Leech - Heal Percent of Enemy Max Health Over Time")]
    [Range(1f, 100f)]
    public float healthPercentPerProc = 2f;

    protected override void Start()
    {
        base.Start();
        
        OnProc(() => {
            if (secondaryTarget) target?.HealPercent(healthPercentPerProc / 100f * secondaryTarget.GetMaxHealth());
        });

        BeginEffect();
    }
}