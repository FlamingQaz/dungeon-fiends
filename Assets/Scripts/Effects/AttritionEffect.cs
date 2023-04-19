using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttritionEffect : Effect
{
    [Header("Attrition - Flat Damage Over Time")]
    public float damagePerProc = 5f;

    protected override void OnProc(ExecutableEffect e)
    {
        e.target?.TakeDamage(damagePerProc, Entity.DamageType.Effect);
    }

    protected override void OnStart(ExecutableEffect e)
    {
        
    }

    protected override void OnEnd(ExecutableEffect e)
    {
        
    }

}
