using System.Collections;
using System.Collections.Generic;

public class AttritionEffect : Effect
{
    public float damagePerProc = 5f;

    protected override void Start()
    {
        base.Start();
        
        OnProc(() => {
            target.TakeDamage(damagePerProc);
        });

        BeginEffect();
    }
}
