using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExecutableEffect
{
    
    UnityEvent<ExecutableEffect> onProcEvent = new UnityEvent<ExecutableEffect>();
    UnityEvent<ExecutableEffect> onStartEvent = new UnityEvent<ExecutableEffect>();
    UnityEvent<ExecutableEffect> onEndEvent = new UnityEvent<ExecutableEffect>();
    bool onCooldown;
    public bool enable;

    /// <summary>This is the Entity receiving the effect.</summary>
    public Entity target;

    /// <summary>If this effect was triggered by an event involving two entities (e.g. on kill), this is the other entity that is not receiving the effect. Not guaranteed to be defined.</summary>
    public Entity secondaryTarget;

    public float secondsPassed;
    public int procs = 0;
    public int stacks = 1;
    public float durationSecs = 10f;
    public float procsPerSec = 1f;
    public bool stackable = false;

    EffectHandler handler;
    public Animator anim;
    public string name;
    public Effect baseEffect;
    public Dictionary<string, object> props = new Dictionary<string, object>();

    public ExecutableEffect(Effect eff) {
        handler = GameObject.FindObjectOfType<EffectHandler>();
        this.baseEffect = eff;
        name = eff.gameObject.name;
        stacks = 1;
        stackable = eff.stackable;
        durationSecs = eff.durationSecs;
        procsPerSec = eff.procsPerSec;

        OnStart((e) => {
            e.anim?.SetBool("hasEffect", true);
        });

        OnEnd((e) => {
            e.anim?.SetBool("hasEffect", false);
        });
    }

    public void Loop()
    {
        // Handle instantaneous effects
        if (enable && durationSecs == 0f) {
            onStartEvent.Invoke(this);
            for (int i = 0; i < stacks; i++) onProcEvent.Invoke(this); // Stack procs if instantaneous
            EndEffect();
            return;
        }

        // End effects once duration is up
        if (secondsPassed == durationSecs) EndEffect();

        // Handle over-time effects
        if (enable && !onCooldown && target) {
            if (secondsPassed == 0f) onStartEvent.Invoke(this);

            // Handle effect with no procs per second
            if (procsPerSec == 0f) {
                procs++;
                onCooldown = true;
                handler.StartCoroutine(NoProcEndCooldown());
                return;
            }

            // Handle effect with procs per second
            onProcEvent.Invoke(this);
            procs++;
            onCooldown = true;
            handler.StartCoroutine(EndCooldown());
        }
    }

    public void OnProc(UnityAction<ExecutableEffect> handler) {
        onProcEvent.AddListener(handler);
    }

    public void OnStart(UnityAction<ExecutableEffect> handler) {
        onStartEvent.AddListener(handler);
    }

    public void OnEnd(UnityAction<ExecutableEffect> handler) {
        onEndEvent.AddListener(handler);
    }

    public void BeginEffect() {
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

        onEndEvent.Invoke(this);
        handler.garbageCollector.Add(this);

        secondsPassed = 0f;
        enable = false;
        secondaryTarget = null;
        onCooldown = false;
    }

    public IEnumerator EndCooldown() {
        yield return new WaitForSeconds(1f/procsPerSec);
        onCooldown = false;
        secondsPassed += 1f/procsPerSec;
    }

    public IEnumerator NoProcEndCooldown() {
        yield return new WaitForSeconds(1f);
        onCooldown = false;
        secondsPassed += 1f;
    }
}
