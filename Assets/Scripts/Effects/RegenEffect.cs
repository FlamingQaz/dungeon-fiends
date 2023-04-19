using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenEffect : Effect
{
    [Header("Regen - Flat Healing Over Time")]
    public float healthPerProc = 5f;

    protected override void OnEnd(ExecutableEffect e)
    {
        
    }

    protected override void OnProc(ExecutableEffect e)
    {
        e.target?.HealFlat(healthPerProc);
    }

    protected override void OnStart(ExecutableEffect e)
    {
       
    }
}
