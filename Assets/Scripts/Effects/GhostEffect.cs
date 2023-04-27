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

    protected override void OnEnd(ExecutableEffect e)
    {
        SpriteRenderer sprite = (SpriteRenderer) e.props["sprite"];
        Color defaultColor = (Color) e.props["defaultColor"];
        float defaultHealth = (float) e.props["defaultHealth"];

        if (e.target) {
            sprite.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
            e.target.SetMaxHealth(defaultHealth);
            e.target.HealPercent(100f);
            if (invisibility) e.target.targetable = true;
        }
    }

    protected override void OnProc(ExecutableEffect e)
    {
        
    }

    protected override void OnStart(ExecutableEffect e)
    {
        // Does not support stacking
        e.stackable = false;

        SpriteRenderer sprite = e.target.GetComponent<SpriteRenderer>();
        Color defaultColor = sprite.color;
        float defaultHealth = e.target.GetMaxHealth();

        e.props.Add("sprite", sprite);
        e.props.Add("defaultColor", defaultColor);
        e.props.Add("defaultHealth", defaultHealth);

        if (e.target) {
            sprite.color = new Color(0, defaultColor.g * (1 - blueShift), 255, defaultColor.a * opacity);
            e.target.SetMaxHealth(defaultHealth * healthChangeFactor);
            e.target.HealPercent(100f);
            if (invisibility) e.target.targetable = false;
        }
    }
}
