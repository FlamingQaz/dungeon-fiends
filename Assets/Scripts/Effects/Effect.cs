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
    UnityEvent onProcEvent = new UnityEvent();
    UnityEvent onStartEvent = new UnityEvent();
    UnityEvent onEndEvent = new UnityEvent();
    bool onCooldown;
    protected bool enable;
    protected Entity target;
    protected float secondsPassed;
    protected int procs = 0;

    protected virtual void Start() {
        if (display) {
            GameObject effectDisplay = Instantiate(display, new Vector2(target.transform.position.x, target.transform.position.y + displayOffsetY), target.transform.rotation);
            effectDisplay.transform.parent = target.transform;
            anim = effectDisplay.GetComponent<Animator>();
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
        secondsPassed = 0f;
        enable = false;
        target = null;
        onCooldown = false;
        onEndEvent.Invoke();
        Destroy(gameObject);
    }

    public void EndCooldown() {
        CancelInvoke(nameof(EndCooldown));
        onCooldown = false;
        secondsPassed += 1f/procsPerSec;
    }

    public void ApplyTo(GameObject targetObj) {
        GameObject effectObj = targetObj.GetComponentInChildren<Effect>()?.gameObject;
        if (effectObj) {
            Effect effect = effectObj.GetComponent<Effect>();
            effect.secondsPassed = 0f;
            return;
        }

        effectObj = Instantiate(gameObject);
        effectObj.transform.parent = targetObj.transform;
        effectObj.GetComponent<Effect>().target = targetObj.GetComponent<Entity>();
    }
}
