using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttritionEffect : Effect
{
    [Header("Attrition - Flat Damage Over Time")]
    public float damagePerProc = 5f;

    protected override void Start()
    {
        base.Start();
        
        OnProc(() => {
            target.TakeDamage(damagePerProc, Entity.DamageType.Effect);
        });

        BeginEffect();
    }
}
