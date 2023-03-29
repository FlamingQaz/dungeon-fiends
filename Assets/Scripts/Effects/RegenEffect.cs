using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenEffect : Effect
{
    public float healthPerProc = 5f;

    protected override void Start()
    {
        base.Start();
        
        OnProc(() => {
            target.HealFlat(healthPerProc);
        });

        BeginEffect();
    }
}
