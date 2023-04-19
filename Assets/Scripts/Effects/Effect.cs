using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A generic benevolent or malevolent effect appliable to any entity
public abstract class Effect : MonoBehaviour
{

    [Tooltip("Duration of the Effect. Set to 0 for an instantaneous effect.")]
    public float durationSecs = 10f;
    public float procsPerSec = 1f;
    public GameObject display;
    public float displayOffsetY = 0.2f;
    public bool stackable = false;

    public ExecutableEffect ApplyTo(GameObject targetObj) {
        List<ExecutableEffect> effectObjs = targetObj.GetComponent<Entity>().CurrentEffects();

        // Handle existing Effects:
        foreach(ExecutableEffect effect in effectObjs) {
            if (effect.name == gameObject.name) {
                if (effect.stackable) {
                    effect.stacks++;
                    if (effect.durationSecs != 0f) effect.secondsPassed -= effect.durationSecs; // Stack effect time if non-instantaneous
                }
                else effect.secondsPassed = 0f;

                return effect;
            }
        }

        // Or create a new one:
        ExecutableEffect eff = new ExecutableEffect(this);
        eff.OnStart(this.OnStart);
        eff.OnProc(this.OnProc);
        eff.OnEnd(this.OnEnd);
        eff.target = targetObj.GetComponent<Entity>();
        if (display) {
            GameObject createdDisplay = eff.target.transform.Find(display.name)?.gameObject;

            if (!createdDisplay) {
                createdDisplay = Instantiate(display, new Vector2(eff.target.transform.position.x, eff.target.transform.position.y + displayOffsetY), eff.target.transform.rotation);
                createdDisplay.name = display.name;
                createdDisplay.transform.parent = eff.target.transform;
            }
            eff.anim = createdDisplay.GetComponent<Animator>();
        }
        targetObj.GetComponent<Entity>().AddEffect(eff);
        FindObjectOfType<EffectHandler>().effectQueue.Add(eff);
        eff.BeginEffect();

        return eff;
    }

    public ExecutableEffect ApplyTo(Entity ent) {
        return ApplyTo(ent.gameObject);
    }

    public void ApplyTo(GameObject targetObj, GameObject secondaryObj) {
        ApplyTo(targetObj).secondaryTarget = secondaryObj.GetComponent<Entity>();
    }

    protected abstract void OnProc(ExecutableEffect e);

    protected abstract void OnStart(ExecutableEffect e);

    protected abstract void OnEnd(ExecutableEffect e);
}
