using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifficultyScaling : MonoBehaviour
{
    public enum Level {
        Any = 0, // Should be equal to lowest Level

        Baby = 0,
        Easy = 1,
        Facile = 2,
        Medium = 3,
        Hard = 5,
        Erect = 8,
        Impossible = 12,
        Eclipse = 17,
        MinigowApocalypse = 100
    }

    bool difficultyChanged = false;
    Level level = Level.Baby;
    Level prevLevel = Level.Baby;
    [SerializeField] Level updatableLevel = Level.Baby;
    [SerializeField] bool scaleAlreadySpawnedEnemies = false;

    public static UnityEvent onChange = new UnityEvent();


    public static Entity.Stats enemyStatScaling = new Entity.Stats {
        maxHealth = 0.33f,
        currentHealth = 0.33f,
        currentShield = 0f, // Do not scale shield
        damage = 0.001f, // Lessen scaling of damage
        movementSpeed = 0f, // Do not scale move speed
        attackSpeed = 0f, // Do not scale attack speed
        resistance = Entity.Stats.WithDefaultAs(0f).resistance // Do not scale resistances
    };


    void FixedUpdate()
    {
        if (updatableLevel != level) SetLevel(updatableLevel);

        if (!difficultyChanged) return;
        difficultyChanged = false;

        HandleScaling();
    }

    public static void SetLevel(Level l) {
        DifficultyScaling scaling = GameObject.FindObjectOfType<DifficultyScaling>();
        scaling.prevLevel = scaling.level;
        scaling.level = l;
        scaling.updatableLevel = l;
        scaling.difficultyChanged = true;
        if (scaling.prevLevel != scaling.level) onChange.Invoke();
    }

    public static Level GetLevel() {
        return GameObject.FindObjectOfType<DifficultyScaling>().level;
    }

    public static Level GetPreviousLevel() {
        return GameObject.FindObjectOfType<DifficultyScaling>().prevLevel;
    }

    void HandleScaling() {
        // ----- For any individual difficulty scaling functionality to trigger on difficulty change -----

        // Enemy:
        if (scaleAlreadySpawnedEnemies) Enemy.SetLocalScaling(GetPreviousLevel(), GetLevel());
    }
}
