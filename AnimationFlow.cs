using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent{
    public delegate void funcDelegate(AnimationFlow animator, Dispatcher dispatcher, float[] x, string y);
    public float timestamp;
    public funcDelegate func;
    public float[] parameters;
    public string text;

    public AnimEvent(float timestamp, funcDelegate function, float[] parameters, string text){
        this.timestamp = timestamp;
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
        animationSequence = sequence0;

        dispatcherRef = simulation.GetComponent<Dispatcher>();
        if(dispatcherRef == null) { print("No simulation dispatcher found!"); }  
        animLength = animationSequence.Length;
        animationStartTime = Time.time;
    }

    void Update()
    {
        while(animCount < animLength){
            if(currentEvent == null) { currentEvent = animationSequence[animCount]; }
            if(currentEvent.timestamp <= Time.time - animationStartTime) {
                currentEvent.func(this, dispatcherRef, currentEvent.parameters, currentEvent.text);
                animCount++;
                currentEvent = animationSequence[animCount];
            } 
            else { break; }
        }
    }
    //params: (duration), type, viewportCoords[2], zoom
    static void SetViewport(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){    
        if(parameters[1] == 1.0f) { animator.StartCoroutine(SetViewportLerp(dispatcher, parameters)); }
        else if(parameters[1] == 2.0f) { animator.StartCoroutine(SetViewportSmooth(dispatcher, parameters)); }
        else {
            if(parameters[2] >= 0.0f) { dispatcher.viewportCoords[0] = parameters[2]; }
            if(parameters[3] >= 0.0f) { dispatcher.viewportCoords[1] = parameters[3]; }
            if(parameters[4] > 0.0f) { dispatcher.zoom = parameters[4]; }
        }     
    }
    //params: duration, (type), viewportCoords[2], zoom
    static IEnumerator SetViewportLerp(Dispatcher dispatcher, float[] parameters){    
        float startTime = Time.time;
        float[] startViewportCoords = new float[2]{dispatcher.viewportCoords[0], dispatcher.viewportCoords[1]};
        float startZoom = dispatcher.zoom;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            if(parameters[2] >= 0.0f){ dispatcher.viewportCoords[0] = Mathf.Lerp(startViewportCoords[0], parameters[2], factor); }
            if(parameters[3] >= 0.0f){ dispatcher.viewportCoords[1] = Mathf.Lerp(startViewportCoords[1], parameters[3], factor); }
            if(parameters[4] > 0.0f) { dispatcher.zoom = Mathf.Lerp(startZoom, parameters[4], factor); }
            yield return null;
        }
        if(parameters[2] >= 0.0f) { dispatcher.viewportCoords[0] = parameters[2]; }
        if(parameters[3] >= 0.0f) { dispatcher.viewportCoords[1] = parameters[3]; }
        if(parameters[4] > 0.0f) { dispatcher.zoom = parameters[4]; }
        yield break;
    }
    //params: duration, (type), viewportCoords[2], zoom
    static IEnumerator SetViewportSmooth(Dispatcher dispatcher, float[] parameters){    
        float startTime = Time.time;
        float[] startViewportCoords = new float[2]{dispatcher.viewportCoords[0], dispatcher.viewportCoords[1]};
        float startZoom = dispatcher.zoom;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            if(parameters[2] >= 0.0f){ dispatcher.viewportCoords[0] = Mathf.SmoothStep(startViewportCoords[0], parameters[2], factor); }
            if(parameters[3] >= 0.0f){ dispatcher.viewportCoords[1] = Mathf.SmoothStep(startViewportCoords[1], parameters[3], factor); }
            if(parameters[4] > 0.0f) { dispatcher.zoom = Mathf.Lerp(startZoom, parameters[4], factor); }
            yield return null;
        }
        if(parameters[2] >= 0.0f) { dispatcher.viewportCoords[0] = parameters[2]; }
        if(parameters[3] >= 0.0f) { dispatcher.viewportCoords[1] = parameters[3]; }
        if(parameters[4] > 0.0f) { dispatcher.zoom = parameters[4]; }
        yield break;
    }
    //params: none
    static void SetRule(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        dispatcher.rule = text;
    }
    //params: none
    static void SetColorScheme(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        dispatcher.colorScheme = text;
    }
    //params: enable
    static void EnableSim(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        dispatcher.enableSim = parameters[0] < 0.5f ? false : true;
    }
    //params: duration, type, step
    static void SetStep(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        if(parameters[1] == 1.0f){ animator.StartCoroutine(SetStepLerp(dispatcher, parameters)); }
        if(parameters[2] == 2.0f){ animator.StartCoroutine(SetStepSmooth(dispatcher, parameters)); }
        else {
            dispatcher.steppingFactor = parameters[2];
        }
    }
    //params: duration, (type), step
    static IEnumerator SetStepLerp(Dispatcher dispatcher, float[] parameters){
        float startTime = Time.time;
        float startStepFactor = dispatcher.steppingFactor;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            dispatcher.steppingFactor = Mathf.Lerp(startStepFactor, parameters[2], factor);
            yield return null;
        }
        dispatcher.steppingFactor = parameters[2];
        yield break;
    }
    //params: duration, (type), step
    static IEnumerator SetStepSmooth(Dispatcher dispatcher, float[] parameters){
        float startTime = Time.time;
        float startStepFactor = dispatcher.steppingFactor;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            dispatcher.steppingFactor = Mathf.SmoothStep(startStepFactor, parameters[2], factor);
            yield return null;
        }
        dispatcher.steppingFactor = parameters[2];
        yield break;
    }
    //params: (duration), type, speed
    static void SetSpeed(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        if(parameters[1] == 1.0f){ animator.StartCoroutine(SetSpeedLerp(dispatcher, parameters)); }
        else if(parameters[1] == 2.0f){ animator.StartCoroutine(SetSpeedSmooth(dispatcher, parameters)); }
        else if(parameters[1] == 3.0f){ animator.StartCoroutine(SetSpeedQuadIn(dispatcher, parameters)); }
        else {
            dispatcher.simSpeed = parameters[2];
        }
    }
    //params: duration, (type), speed
    static IEnumerator SetSpeedLerp(Dispatcher dispatcher, float[] parameters){
        float startTime = Time.time;
        float startSpeed = dispatcher.simSpeed;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            dispatcher.simSpeed = Mathf.Lerp(startSpeed, parameters[2], factor);
            yield return null;
        }
        dispatcher.simSpeed = parameters[2];
        yield break;
    }
    //params: duration, (type), speed
    static IEnumerator SetSpeedSmooth(Dispatcher dispatcher, float[] parameters){
        float startTime = Time.time;
        float startSpeed = dispatcher.simSpeed;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            dispatcher.simSpeed = Mathf.SmoothStep(startSpeed, parameters[2], factor);
            yield return null;
        }
        dispatcher.simSpeed = parameters[2];
        yield break;
    }
    //params: duration, (type), speed
    static IEnumerator SetSpeedQuadIn(Dispatcher dispatcher, float[] parameters){
        float startTime = Time.time;
        float startSpeed = dispatcher.simSpeed;
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            factor =  1.0f - ((1.0f - factor) * (1.0f - factor));
            dispatcher.simSpeed = Mathf.Lerp(startSpeed, parameters[2], factor);
            yield return null;
        }
        dispatcher.simSpeed = parameters[2];
        yield break;
    }
    //params: (duration), type, channel, color[4]
    static void SetColor(AnimationFlow animator, Dispatcher dispatcher, float[] parameters, string text){
        int channelOffset = (int)parameters[2] * 4;
        if(parameters[1] == 1.0f) { animator.StartCoroutine(SetColorLerp(dispatcher, parameters)); }
        else if(parameters[1] == 2.0f) { animator.StartCoroutine(SetColorSmooth(dispatcher, parameters)); }
        else {
            if(parameters[3] >= 0.0f) { dispatcher.colors[channelOffset] = parameters[3]; }
            if(parameters[4] >= 0.0f) { dispatcher.colors[channelOffset + 1] = parameters[4]; }
            if(parameters[5] >= 0.0f) { dispatcher.colors[channelOffset + 2] = parameters[5]; }
            if(parameters[6] >= 0.0f) { dispatcher.colors[channelOffset + 3] = parameters[6]; }
        }
    }
    //params: duration, type, channel, color[4]
    static IEnumerator SetColorLerp(Dispatcher dispatcher, float[] parameters){
        int channelOffset = (int)parameters[2] * 4;
        float startTime = Time.time;
        float[] startColor = new float[4]{  dispatcher.colors[channelOffset], 
                                            dispatcher.colors[channelOffset + 1], 
                                            dispatcher.colors[channelOffset + 2], 
                                            dispatcher.colors[channelOffset + 3]};
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            if(parameters[3] >= 0.0f){ dispatcher.colors[channelOffset] = Mathf.Lerp(startColor[0], parameters[3], factor); }
            if(parameters[4] >= 0.0f){ dispatcher.colors[channelOffset + 1] = Mathf.Lerp(startColor[1], parameters[4], factor); }
            if(parameters[5] >= 0.0f){ dispatcher.colors[channelOffset + 2] = Mathf.Lerp(startColor[2], parameters[5], factor); }
            if(parameters[6] >= 0.0f){ dispatcher.colors[channelOffset + 3] = Mathf.Lerp(startColor[3], parameters[6], factor); }
            yield return null;
        }
        if(parameters[3] >= 0.0f) { dispatcher.colors[channelOffset] = parameters[3]; }
        if(parameters[4] >= 0.0f) { dispatcher.colors[channelOffset + 1] = parameters[4]; }
        if(parameters[5] >= 0.0f) { dispatcher.colors[channelOffset + 2] = parameters[5]; }
        if(parameters[6] >= 0.0f) { dispatcher.colors[channelOffset + 3] = parameters[6]; }
        yield break;
    }
    //params: duration, (type), channel, color[4]
    static IEnumerator SetColorSmooth(Dispatcher dispatcher, float[] parameters){
        int channelOffset = (int)parameters[2] * 4;
        float startTime = Time.time;
        float[] startColor = new float[4]{  dispatcher.colors[channelOffset], 
                                            dispatcher.colors[channelOffset + 1], 
                                            dispatcher.colors[channelOffset + 2], 
                                            dispatcher.colors[channelOffset + 3]};
        float factor;
        while(startTime + parameters[0] > Time.time){
            factor = (Time.time - startTime) / parameters[0];
            if(parameters[3] >= 0.0f){ dispatcher.colors[channelOffset] = Mathf.SmoothStep(startColor[0], parameters[3], factor); }
            if(parameters[4] >= 0.0f){ dispatcher.colors[channelOffset + 1] = Mathf.SmoothStep(startColor[1], parameters[4], factor); }
            if(parameters[5] >= 0.0f){ dispatcher.colors[channelOffset + 2] = Mathf.SmoothStep(startColor[2], parameters[5], factor); }
            if(parameters[6] >= 0.0f){ dispatcher.colors[channelOffset + 3] = Mathf.SmoothStep(startColor[3], parameters[6], factor); }
            yield return null;
        }
        if(parameters[3] >= 0.0f) { dispatcher.colors[channelOffset] = parameters[3]; }
        if(parameters[4] >= 0.0f) { dispatcher.colors[channelOffset + 1] = parameters[4]; }
        if(parameters[5] >= 0.0f) { dispatcher.colors[channelOffset + 2] = parameters[5]; }
        if(parameters[6] >= 0.0f) { dispatcher.colors[channelOffset + 3] = parameters[6]; }
        yield break;
    }



    AnimEvent[] sequence1 = new AnimEvent[]
    {
        new AnimEvent(2.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };

    AnimEvent[] sequence0 = new AnimEvent[]
    {
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, -1, -1, 300.0f}, ""),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.1f}, ""),
        new AnimEvent(2.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(3.0f, SetColor, new float[]{-1, 0, 1, 0.0f, 0.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(4.0f, SetViewport, new float[]{2.0f, 2, 200.0f, 100.0f, 200.0f}, ""),
        new AnimEvent(5.0f, SetColor, new float[]{2.0f, 1, 1, 1.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(6.0f, SetRule, new float[]{}, "B2"),
        new AnimEvent(8.0f, SetStep, new float[]{5.0f, 1, 0.2f}, ""),
        new AnimEvent(8.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(10.0f, SetSpeed, new float[]{10.0f, 3, 0.01f}, ""),
        new AnimEvent(10.0f, SetColor, new float[]{4.0f, 2, 2, 0.0f, 0.7f, 0.7f, 1.0f}, ""),
        new AnimEvent(14.0f, SetColorScheme, new float[]{}, "Raw"),
        new AnimEvent(99999.0f, SetRule, new float[]{}, ""), //this is just pure jank lol
    };
}
