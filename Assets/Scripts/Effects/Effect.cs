using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// A generic benevolent or malevolent effect appliable to any entity
public class Effect : MonoBehaviour
{

    [Tooltip("Duration of the Effect. Set to 0 for an instantaneous effect.")]
    public float durationSecs = 10f;
    public float procsPerSec = 1f;
    Animator anim;
    public GameObject display;
    public float displayOffsetY = 0.2f;
    public bool stackable = false;
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
    protected int stacks = 0;

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
        // Handle instantaneous effects
        if (enable && durationSecs == 0f) {
            onStartEvent.Invoke();
            for (int i = 0; i < stacks; i++) onProcEvent.Invoke(); // Stack procs if instantaneous
            EndEffect();
            return;
        }

        // End effects once duration is up
        if (secondsPassed == durationSecs) EndEffect();

        // Handle over-time effects
        if (enable && !onCooldown && target) {
            if (secondsPassed == 0f) onStartEvent.Invoke();

            // Handle effect with no procs per second
            if (procsPerSec == 0f) {
                procs++;
                onCooldown = true;
                Invoke(nameof(NoProcEndCooldown), 1f);
                return;
            }

            // Handle effect with procs per second
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

        onEndEvent.Invoke();
        secondsPassed = 0f;
        enable = false;
        target = null;
        secondaryTarget = null;
        onCooldown = false;

        Destroy(createdDisplay);
        Destroy(gameObject);
    }

    public void EndCooldown() {
        CancelInvoke(nameof(EndCooldown));
        onCooldown = false;
        secondsPassed += 1f/procsPerSec;
    }

    public void NoProcEndCooldown() {
        CancelInvoke(nameof(NoProcEndCooldown));
        onCooldown = false;
        secondsPassed += 1f;
    }

    public Effect ApplyTo(GameObject targetObj) {
        Effect[] effectObjs = targetObj.GetComponent<Entity>().CurrentEffects();

        // Handle existing Effects:
        foreach(Effect effect in effectObjs) {
            if (effect.gameObject.name == gameObject.name) {
                if (effect.stackable) {
                    effect.stacks++;
                    if (effect.durationSecs != 0f) effect.secondsPassed -= effect.durationSecs; // Stack effect time if non-instantaneous
                }
                else effect.secondsPassed = 0f;

                return effect;
            }
        }

        // Or create a new one:
        GameObject effectObj = Instantiate(gameObject);
        effectObj.name = gameObject.name;
        effectObj.transform.parent = targetObj.transform;
        Effect eff = effectObj.GetComponent<Effect>();
        eff.target = targetObj.GetComponent<Entity>();
        eff.stacks = 1;

        return effectObj.GetComponent<Effect>();
    }

    public Effect ApplyTo(Entity ent) {
        return ApplyTo(ent.gameObject);
    }

    public void ApplyTo(GameObject targetObj, GameObject secondaryObj) {
        ApplyTo(targetObj).secondaryTarget = secondaryObj.GetComponent<Entity>();
    }
}
