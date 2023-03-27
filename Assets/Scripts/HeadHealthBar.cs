using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health bar that shows above entity heads
public class HeadHealthBar : MonoBehaviour
{
    SpriteRenderer sprite;

    public void Set(float health, float maxHealth) {
        if (maxHealth == 0f) maxHealth = 0.1f;

        if (!sprite) sprite = GetComponent<SpriteRenderer>();
        sprite.size = new Vector2(health/maxHealth, sprite.size.y);
    }
}
