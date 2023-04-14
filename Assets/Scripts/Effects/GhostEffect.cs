using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : Effect
{
    [Header("Ghost - Enter a ghost form for a limited time")]
    public float opacity = 0.3f;
    public float blueShift = 0.45f;
    public float healthChangeFactor = 1.5f;
    public bool invisibility = false;

    protected override void Start()
    {
        base.Start();
        stackable = false; // Ghost does not support stacking

        SpriteRenderer sprite = target.GetComponent<SpriteRenderer>();
        Color defaultColor = sprite.color;
        float defaultHealth = target.GetMaxHealth();
        
        OnStart(() => {
            if (target) {
                sprite.color = new Color(0, defaultColor.g * (1 - blueShift), 255, defaultColor.a * opacity);
                target.SetMaxHealth(defaultHealth * healthChangeFactor);
                target.HealPercent(100f);
                if (invisibility) target.targetable = false;
            }
        });

        OnEnd(() => {
            if (target) {
                sprite.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                target.SetMaxHealth(defaultHealth);
                target.HealPercent(100f);
                if (invisibility) target.targetable = true;
            }
        });

        BeginEffect();
    }
}
