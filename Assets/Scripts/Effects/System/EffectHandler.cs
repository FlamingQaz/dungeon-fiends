using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    protected List<ExecutableEffect> effects = new List<ExecutableEffect>();
    public List<ExecutableEffect> garbageCollector = new List<ExecutableEffect>();
    public List<ExecutableEffect> effectQueue = new List<ExecutableEffect>();

    protected void FixedUpdate()
    {
        foreach (ExecutableEffect effect in effects) {
            effect.Loop();
        }

        foreach(ExecutableEffect effect in garbageCollector) {
            effects.Remove(effect);
            effect.target.RemoveEffect(effect);
        }
        garbageCollector.Clear();

        foreach(ExecutableEffect effect in effectQueue) {
            effects.Add(effect);
        }
        effectQueue.Clear();
    }
}
