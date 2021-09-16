using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent{
    public delegate void funcDelegate(AnimationFlow animator, Dispatcher dispatcher, float t, float[] x, string y);

    public float timestamp;
    public float duration;
    public funcDelegate func;
    public float[] parameters;
    public string text;

    public AnimEvent(float timestamp, float duration, funcDelegate function, float[] parameters, string text){
        this.timestamp = timestamp;
        this.duration = duration;
        this.func = function;
        this.parameters = parameters;
        this.text = text;
    }
}

public class AnimationFlow : MonoBehaviour
{
    public GameObject simulation;
    private Dispatcher dispatcherRef;

    private AnimEvent[] animationSequence;
    private AnimEvent currentEvent;

    private float animationStartTime;
    private int animCount = 0;
    private int animLength;

    void Start()
    {
        dispatcherRef = simulation.GetComponent<Dispatcher>();
        if(dispatcherRef == null) { print("No simulation dispatcher found!"); }
        animationSequence = sequence0;
        animLength = animationSequence.Length;
        animationStartTime = Time.time;
    }

    void Update()
    {
        while(animCount < animLength){
            if(currentEvent == null) { currentEvent = animationSequence[animCount]; }
            if(currentEvent.timestamp <= Time.time - animationStartTime) {
                currentEvent.func(this, dispatcherRef, currentEvent.duration, currentEvent.parameters, currentEvent.text);
                print("executedanimation");
                animCount++;
                currentEvent = animationSequence[animCount];
            } 
            else { break; }
        }
    }

    static void SetViewport(AnimationFlow animator, Dispatcher dispatcher, float duration, float[] parameters, string text){    //params: viewportCoords[2], zoom, centerviewport[2]
        dispatcher.centerViewport[0] = parameters[3] < 0.0f ? dispatcher.centerViewport[0] : parameters[3] >= 1.0f ? true : false;
        dispatcher.centerViewport[1] = parameters[4] < 0.0f ? dispatcher.centerViewport[1] : parameters[4] >= 1.0f ? true : false;
        if(duration >= 0.0f) { animator.StartCoroutine(SetViewportLerp(dispatcher, duration, parameters)); }
        else {
            if(parameters[0] >= 0.0f) { dispatcher.viewportCoords[0] = parameters[0]; }
            if(parameters[1] >= 0.0f) { dispatcher.viewportCoords[1] = parameters[1]; }
            if(parameters[2] > 0.0f) { dispatcher.zoom = parameters[2]; }
        }     
    }

    static IEnumerator SetViewportLerp(Dispatcher dispatcher, float duration, float[] parameters){    //params: viewportCoords[2], zoom, centerviewport[2]
        float startTime = Time.time;
        float[] startViewportCoords = new float[2]{dispatcher.viewportCoords[0], dispatcher.viewportCoords[1]};
        float startZoom = dispatcher.zoom;
        while(startTime + duration > Time.time){
            float fac = duration / (Time.time - startTime);
            if(parameters[0] >= 0.0f){ dispatcher.viewportCoords[0] = Mathf.Lerp(startViewportCoords[0], parameters[0], fac); }
            if(parameters[1] >= 0.0f){ dispatcher.viewportCoords[1] = Mathf.Lerp(startViewportCoords[1], parameters[1], fac); }
            if(parameters[2] > 0.0f) { Mathf.Lerp(startZoom, parameters[2], fac); }
            yield return null;
        }
        if(parameters[0] >= 0.0f) { dispatcher.viewportCoords[0] = parameters[0]; }
        if(parameters[1] >= 0.0f) { dispatcher.viewportCoords[1] = parameters[1]; }
        if(parameters[2] > 0.0f) { dispatcher.zoom = parameters[2]; }
    }

    static void SetRule(AnimationFlow animator, Dispatcher dispatcher, float duration, float[] parameters, string text){
        dispatcher.rule = text;
    }

    static void EnableSim(AnimationFlow animator, Dispatcher dispatcher, float duration, float[] parameters, string text){
        if(parameters[0] > 0.0f){ dispatcher.enableSim = parameters[0] < 0.5f ? false : true; }
    }

    AnimEvent[] sequence0 = new AnimEvent[]
    {
        new AnimEvent(0.0f, -1.0f, SetViewport, new float[]{-1.0f, -1.0f, 300.0f, -1.0f, -1.0f}, ""),
        new AnimEvent(2.0f, -1.0f, EnableSim, new float[]{1.0f}, ""),
        new AnimEvent(4.0f, -1.0f, SetViewport, new float[]{200.0f, 100.0f, 200.0f, -1.0f, -1.0f}, ""),
        new AnimEvent(6.0f, -1.0f, SetRule, new float[]{}, "B2"),
        new AnimEvent(99999.0f, -1.0f, SetRule, new float[]{}, ""), //this is just pure jank lol
    };
}
