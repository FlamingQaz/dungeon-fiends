using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// A generic benevolent or malevolent effect appliable to any entity
public class Effect : MonoBehaviour
{

    public float durationSecs = 10f;
    public float procsPerSec = 1f;
    Animator anim;
    public GameObject display;
    public float displayOffsetY = 0.2f;
    GameObject createdDisplay;
    UnityEvent onProcEvent = new UnityEvent();
    UnityEvent onStartEvent = new UnityEvent();
    UnityEvent onEndEvent = new UnityEvent();
    bool onCooldown;
    protected bool enable;

    /// <summary>This is the Entity receiving the effect.</summary>
    protected Entity target;

    /// <summary>If this effect was triggered by an event involving two entities (e.g. on kill), this is the other entity that is not receiving the effect. Not guaranteed to be defined.</summary>
    protected Entity secondaryTarget;

    protected float secondsPassed;
    protected int procs = 0;

    protected virtual void Start() {
        if (display) {
            createdDisplay = Instantiate(display, new Vector2(target.transform.position.x, target.transform.position.y + displayOffsetY), target.transform.rotation);
            createdDisplay.name = display.name;
            createdDisplay.transform.parent = target.transform;
            anim = createdDisplay.GetComponent<Animator>();
        }

        OnStart(() => {
            anim?.SetBool("hasEffect", true);
        });

        OnEnd(() => {
            anim?.SetBool("hasEffect", false);
        });
    }

    void FixedUpdate()
    {
        if (secondsPassed == durationSecs) EndEffect();

        if (enable && !onCooldown && target) {
            if (secondsPassed == 0f) onStartEvent.Invoke();

            onProcEvent.Invoke();
            procs++;
            onCooldown = true;
            Invoke(nameof(EndCooldown), 1f/procsPerSec);
        }
    }

    protected void OnProc(UnityAction handler) {
        onProcEvent.AddListener(handler);
    }

    protected void OnStart(UnityAction handler) {
        onStartEvent.AddListener(handler);
    }

    protected void OnEnd(UnityAction handler) {
        onEndEvent.AddListener(handler);
    }

    protected void BeginEffect() {
        onCooldown = false;
        secondsPassed = 0f;
        procs = 0;
        enable = true;
    }

    public void EndEffect() {
        if (secondaryTarget && target && !target.isAlive && !target.onKillTriggered) {
            secondaryTarget.onKill.Invoke(target);
            target.onKillTriggered = true;
        }

        secondsPassed = 0f;
        enable = false;
        target = null;
        secondaryTarget = null;
        onCooldown = false;
        onEndEvent.Invoke();

        Destroy(createdDisplay);
        Destroy(gameObject);
    }

    public void EndCooldown() {
        CancelInvoke(nameof(EndCooldown));
        onCooldown = false;
        secondsPassed += 1f/procsPerSec;
    }

    public Effect ApplyTo(GameObject targetObj) {
        Effect[] effectObjs = targetObj.GetComponent<Entity>().CurrentEffects();

        // Extend existing Effects:
        foreach(Effect effect in effectObjs) {
            if (effect.gameObject.name == gameObject.name) {
                effect.secondsPassed = 0f;
                return effect;
            }
        }

        // Or create a new one:
        GameObject effectObj = Instantiate(gameObject);
        effectObj.name = gameObject.name;
        effectObj.transform.parent = targetObj.transform;
        effectObj.GetComponent<Effect>().target = targetObj.GetComponent<Entity>();

        return effectObj.GetComponent<Effect>();
    }

    public Effect ApplyTo(Entity ent) {
        return ApplyTo(ent.gameObject);
    }

    public void ApplyTo(GameObject targetObj, GameObject secondaryObj) {
        ApplyTo(targetObj).secondaryTarget = secondaryObj.GetComponent<Entity>();
    }
}
