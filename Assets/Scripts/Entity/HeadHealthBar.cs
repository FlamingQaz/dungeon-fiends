using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health bar that shows above entity heads
public class HeadHealthBar : MonoBehaviour
{
    SpriteRenderer healthSprite;
    SpriteRenderer shieldSprite;

    public void Set(float health, float maxHealth, float shield) {
        if (maxHealth == 0f) maxHealth = 0.1f;

        if (!healthSprite) healthSprite = GetComponent<SpriteRenderer>();
        if (!shieldSprite) shieldSprite = transform.parent.GetComponentInChildren<SpriteRenderer>();
        healthSprite.size = new Vector2(health/maxHealth, healthSprite.size.y);
        shieldSprite.size = new Vector2(shield/maxHealth, shieldSprite.size.y);
    }
}
