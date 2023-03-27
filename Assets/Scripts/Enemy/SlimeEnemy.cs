using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class SlimeEnemy : MonoBehaviour
{

    [HideInInspector] public Enemy enemy;
    public float maxScale = 3;
    public float startScale = 2;
    float scale;
    bool mergeable = true;
    bool scaled = false;
    bool splitting = false;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (!scaled) {
            scale = startScale;
            SetScale();
        }
    }

    public void SetScale() {
        float value;
        if (scale <= startScale) value = 1 / (startScale - scale + 1);
        else value = scale / startScale;

        transform.localScale = new Vector2(value, value);
        scaled = true;
    }

    public void Init(float newScale) {
        scale = newScale;
        SetScale();
        mergeable = false;
        
        float value;
        if (scale <= startScale) value = 1 / (startScale - scale + 1);
        else value = scale / startScale;
        enemy.entity.SetMaxHealth(enemy.entity.GetMaxHealth() * value);
        enemy.entity.HealPercent(100f);
    }

    public void Split() {
        if (scale - 1 < 0f) return;
        if (enemy.entity.isAlive || splitting) return;

        splitting = true;
        float amount = Mathf.Pow(2, startScale - (scale - 1));
        for (int i = (int) amount; i > 0; i--) {
            Instantiate(gameObject, (Vector2)transform.position + new Vector2(1f/i, 1f/i), transform.rotation).GetComponent<SlimeEnemy>().Init(scale - 1);
        }
    }

    public void Merge(GameObject other) {
        if (!mergeable || scale + 1 > maxScale) return;

        Destroy(other);
        Instantiate(gameObject, transform.position, transform.rotation).GetComponent<SlimeEnemy>().Init(scale + 1);
    }
}
