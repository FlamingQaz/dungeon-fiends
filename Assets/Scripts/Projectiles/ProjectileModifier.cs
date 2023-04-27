using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModifier : MonoBehaviour
{

    public enum EffectTarget {
        ProjectileShooter,
        ProjectileTarget
    }

    [System.Serializable]
    public struct Modifier {
        public Effect effect;
        public EffectTarget target;
    }

    public List<Modifier> modifiers;
    BasicProjectile projectile;

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponent<BasicProjectile>();

        foreach (Modifier modifier in modifiers) {
            Effect effect = modifier.effect;
            EffectTarget targetType = modifier.target;

            projectile.onHit.AddListener((Entity target) => {
                if (targetType == EffectTarget.ProjectileShooter) effect.ApplyTo(projectile.shooter.gameObject);
                else if (targetType == EffectTarget.ProjectileTarget) effect.ApplyTo(target.gameObject);
            });
        }
    }
}
