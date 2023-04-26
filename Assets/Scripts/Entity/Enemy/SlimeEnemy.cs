using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class SlimeEnemy : MonoBehaviour
{

    [HideInInspector] public Enemy enemy;
    public float maxScale = 3f;
    public float startScale = 2f;
    public float mergeDelay = 10f; // Seconds before merging can occur again
    public float mergeRange = 2f; // Range to check for other slimes to merge with
    float scale;
    bool mergeable = false; // Whether this slime can currently merge
    bool scaled = false; // Whether this slime's proper scale has been set
    bool splitted = false; // Whether this slime has died and split into new slimes
    BoxCollider2D bc;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        enemy = GetComponent<Enemy>();
        if (!scaled) {
            scale = startScale;
            SetScale();
        }

        Invoke(nameof(EndMergeCooldown), mergeDelay);
    }

    void FixedUpdate() {
        if (!mergeable || scale + 1 > maxScale) return;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, mergeRange, Vector2.zero, 0f);
        foreach (RaycastHit2D hit in hits) {
            if (!hit) continue;

            SlimeEnemy otherSlime = hit.transform.gameObject.GetComponent<SlimeEnemy>();
            if (!otherSlime || otherSlime.gameObject == gameObject || !otherSlime.mergeable || scale != otherSlime.scale) continue;
            if (GetInstanceID() < otherSlime.GetInstanceID()) continue; // Prevents two slimes from both trying to merging with each other at the same time

            Merge(otherSlime);
            return;
        }

        mergeable = false;
        Invoke(nameof(EndMergeCooldown), mergeDelay);
    }

    public void SetScale() {
        float value = GetScaleModifier(scale);

        transform.localScale = new Vector2(value, value);
        scaled = true;
    }

    public GameObject Init(float newScale) {
        scale = newScale;
        SetScale();
        
        float value = GetScaleModifier(scale);
        enemy.entity.SetMaxHealth(enemy.entity.GetRawStats().maxHealth * value);
        enemy.entity.HealPercent(100f);

        return gameObject;
    }

    public void Split() {
        if (scale - 1 < 0f) return;
        if (enemy.entity.isAlive || splitted) return;

        splitted = true;
        float amount = Mathf.Pow(2, startScale - (scale - 1));
        for (int i = (int) amount; i > 0; i--) {
            enemy.Clone(GetRandomPosition()).GetComponent<SlimeEnemy>().Init(scale - 1).name = gameObject.name.Replace(" (Split)", "").Replace(" (Merged)", "") + " (Split)";
        }
    }

    public void Merge(SlimeEnemy other) {
        Destroy(other.gameObject);
        SlimeEnemy newSlime = enemy.Clone().GetComponent<SlimeEnemy>();

        float value = GetScaleModifier(scale);

        newSlime.Init(scale + 1);
        newSlime.enemy.entity.SetMaxHealth(enemy.entity.GetRawStats().maxHealth / value);
        newSlime.enemy.entity.HealPercent(100f);

        newSlime.gameObject.name = gameObject.name.Replace(" (Split)", "").Replace(" (Merged)", "") + " (Merged)";
        Destroy(gameObject);
    }

    Vector2 GetRandomPosition() {
        Vector2 rectSize = new Vector2();
        rectSize.x = transform.localScale.x * bc.size.x;
        rectSize.y = transform.localScale.y * bc.size.y;

        Vector2 newPos = new Vector2(Random.Range(-rectSize.x / 2, rectSize.x / 2), Random.Range(-rectSize.y / 2, rectSize.y / 2));
 
        return (Vector2)transform.position + newPos;
    }

    protected void EndMergeCooldown() {
        mergeable = true;
    }
    
    float GetScaleModifier(float scaleValue) {
        float value;
        if (scaleValue <= startScale) value = 1 / (startScale - scaleValue + 1);
        else value = scaleValue / startScale;

        return value;
    }
}
