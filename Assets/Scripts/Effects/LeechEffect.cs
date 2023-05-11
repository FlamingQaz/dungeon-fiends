using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechEffect : Effect
{
    [Header("Leech - Heal Percent of Enemy Max Health Over Time")]
    [Range(1f, 100f)]
    public float healthPercentPerProc = 2f;

    protected override void OnEnd(ExecutableEffect e)
    {
        
    }

    protected override void OnProc(ExecutableEffect e)
    {
        if (e.secondaryTarget) e.target?.HealPercent(healthPercentPerProc / 100f * e.secondaryTarget.GetMaxHealth());
    }

    protected override void OnStart(ExecutableEffect e)
    {
        
    }
}