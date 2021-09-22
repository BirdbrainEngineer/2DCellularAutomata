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
            if(parameters[4] > 0.0f) { dispatcher.zoom = Mathf.SmoothStep(startZoom, parameters[4], factor); }
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


    static public float spb = 0.82f; //seconds per beat
    //SetViewport: (duration), type, viewportCoords[2], zoom
    //SetRule: -
    //SetColorScheme: -
    //EnableSim: enable
    //SetStep: duration, type, step
    //SetSpeed: (duration), type, speed
    //SetColors: (duration), type, channel, color[4]
    //Vanilla, Lerp, Raw, Ratio, 

    //randomsequence2
    AnimEvent[] sequence0 = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "S34678B3678"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.05f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 0.05f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Lerp"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 1.0f, 0.5f, 0.5f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.9f, 0.1f, 0.05f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 1024, 1024, 1080}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{28 * spb, 1, 1024, 1024, 512 + 256}, ""),

        new AnimEvent(1.0f + 4 * spb, SetRule, new float[]{}, "S012345678B3"),
        new AnimEvent(1.0f + 4 * spb, SetColor, new float[]{spb * 4, 1, 0, 1.0f, 1.0f, 0.5f, 1.0f}, ""),
        new AnimEvent(1.0f + 4 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.8f, 0.8f, 0.1f, 1.0f}, ""),
        new AnimEvent(1.0f + 4 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.15f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 8 * spb, SetRule, new float[]{}, "S2345B45678"),
        new AnimEvent(1.0f + 8 * spb, SetColor, new float[]{spb * 4, 1, 0, 0.5f, 1.0f, 0.5f, 1.0f}, ""),
        new AnimEvent(1.0f + 8 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.1f, 0.9f, 0.1f, 1.0f}, ""),
        new AnimEvent(1.0f + 8 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.15f, 0.15f, 0.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetRule, new float[]{}, "S1234B3"),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{spb * 4, 1, 0, 0.5f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.05f, 0.8f, 0.8f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.0f, 0.15f, 0.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 16 * spb, SetRule, new float[]{}, "S45678B3"),
        new AnimEvent(1.0f + 16 * spb, SetColor, new float[]{spb * 4, 1, 0, 0.5f, 0.5f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 16 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.05f, 0.1f, 0.9f, 1.0f}, ""),
        new AnimEvent(1.0f + 16 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.0f, 0.15f, 0.15f, 1.0f}, ""),
        new AnimEvent(1.0f + 20 * spb, SetRule, new float[]{}, "S012345678B3"),
        new AnimEvent(1.0f + 20 * spb, SetColor, new float[]{spb * 4, 1, 0, 1.0f, 0.5f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 20 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.7f, 0.05f, 0.9f, 1.0f}, ""),
        new AnimEvent(1.0f + 20 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.0f, 0.0f, 0.15f, 1.0f}, ""),
        new AnimEvent(1.0f + 20 * spb, SetSpeed, new float[]{-1, 0, 0.0033f}, ""),
        new AnimEvent(1.0f + 24 * spb, SetRule, new float[]{}, "S1358B357"),
        new AnimEvent(1.0f + 24 * spb, SetColor, new float[]{spb * 4, 1, 0, 1.0f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 24 * spb, SetColor, new float[]{spb * 4, 1, 1, 0.7f, 0.7f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 24 * spb, SetColor, new float[]{spb * 4, 1, 2, 0.15f, 0.0f, 0.15f, 1.0f}, ""),
        new AnimEvent(1.0f + 24 * spb, SetSpeed, new float[]{-1, 0, 0.02f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(1.0f + 28 * spb, SetSpeed, new float[]{spb * 8, 1, 1.0f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetStep, new float[]{spb * 8, 1, 1.0f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetViewport, new float[]{spb * 8, 1, 1107, 888, 15}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //birthdaypuffersequence
    AnimEvent[] birthdaypuffersequence = new AnimEvent[]
    {
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.004f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 0.05f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Lerp"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.9f, 0.5f, 0.95f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.8f, 0.0f, 0.9f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 30, 930, 128}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetStep, new float[]{spb * 16, 1, 0.02f}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{spb * 16, 1, 400, 450, 512}, ""),
        new AnimEvent(1.0f, SetColor, new float[]{spb * 16, 1, 0, 1.1f, 0.7f, 1.2f, 1.0f}, ""),
        new AnimEvent(1.0f, SetColor, new float[]{spb * 16, 1, 1, 1.0f, 0.0f, 1.1f, 1.0f}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //centerseedsequence
    AnimEvent[] centerseedsequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B37S12345"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.51f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Lerp"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.0025f, 0.1f, 0.98f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.3f, 1.0f, 0.14f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 1000, 1080, 256}, ""),

        new AnimEvent(0.0f, EnableSim, new float[]{1}, ""),

        new AnimEvent(1.0f, SetStep, new float[]{-1, 0, 0.0023f}, ""),
        new AnimEvent(1.0f, SetSpeed, new float[]{-1, 0, 0.0065f}, ""),
        new AnimEvent(1.0f + spb * 4, SetViewport, new float[]{-1, 0, 1000, 1450, 1920}, ""),
        new AnimEvent(1.0f + spb * 4, SetSpeed, new float[]{-1, 0, 0.002f}, ""),

        new AnimEvent(1.0f + 11 * spb, SetColor, new float[]{spb, 1, 0, 0.8f, 0.9f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 11 * spb, SetColor, new float[]{spb, 1, 1, 0.3f, 1.0f, 0.14f, 1.0f}, ""),
        new AnimEvent(1.0f + 11 * spb, SetColor, new float[]{spb, 1, 2, 0.4f, 0.4f, 0.4f, 1.0f}, ""),

        new AnimEvent(1.0f + 12 * spb, SetRule, new float[]{}, "S34678B3678"),
        new AnimEvent(1.0f + 12 * spb, SetSpeed, new float[]{-1, 0, 0.001f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{0.35f, 1, 0, 0.7f, 0.8f, 0.9f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{0.35f, 1, 1, 0.6f, 0.05f, 0.75f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{0.35f, 1, 2, 0.08f, 0.0f, 0.05f, 1.0f}, ""),
        new AnimEvent(1.0f + 12 * spb, SetColor, new float[]{spb * 8, 1, 1, 0.9f, 0.02f, 0.15f, 1.0f}, ""),

        new AnimEvent(1.0f + 20 * spb, SetRule, new float[]{}, "S45678B3"),
        new AnimEvent(1.0f + 20 * spb, SetSpeed, new float[]{-1, 0, 0.0033f}, ""),
        new AnimEvent(1.0f + 20 * spb, SetColor, new float[]{spb * 2, 1, 2, 0.08f, 0.0f, 0.7f, 1.0f}, ""),

        new AnimEvent(1.0f + 28 * spb, SetRule, new float[]{}, "S1358B357"),
        new AnimEvent(1.0f + 28 * spb, SetColorScheme, new float[]{}, "Raw"),
        new AnimEvent(1.0f + 28 * spb, SetSpeed, new float[]{-1, 0, 0.01f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetColor, new float[]{0.35f, 1, 0, 0.1f, 0.7f, 0.1f, 1.0f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetColor, new float[]{0.35f, 1, 1, 0.76f, 0.85f, 0.11f, 1.0f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetColor, new float[]{0.35f, 1, 2, 0.3f, 0.1f, 0.7f, 1.0f}, ""),
        new AnimEvent(1.0f + 28 * spb, SetStep, new float[]{-1, 0, 0.1f}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //randomsequence
    AnimEvent[] randomsequence = new AnimEvent[]
    {
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.05f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Raw"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.7f, 1.4f, 0.65f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.3f, 0.4f, 1.1f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 128, 128, 256}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetSpeed, new float[]{spb * 7, 1, 0.012f}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{20.0f, 1, 0, 0, 512}, ""),
        new AnimEvent(1.0f, SetStep, new float[]{spb * 6, 1, 0.1f}, ""),
        new AnimEvent(1.0f + spb * 8, SetRule, new float[]{}, "B34S34"),
        new AnimEvent(1.0f + spb * 8, SetColor, new float[]{3.0f, 1, 1, 1.5f, 1.2f, 0.2f, 1.0f}, ""),
        new AnimEvent(1.0f + spb * 8, SetColor, new float[]{6.0f, 1, 0, 0.4f, 1.0f, 0.9f, 1.0f}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //vgunsequence
    AnimEvent[] vgunsequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.005f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 0.1f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Raw"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 1.8f, 0.5f, 0.65f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.7f, 0.8f, 0.2f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 400, 3800, 512}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{spb * 14, 2, 1650, 2650, 1024}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //galaxysequence
    AnimEvent[] galaxysequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.2f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Lerp"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.9f, 0.9f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.6f, 0.4f, 0.7f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 63, 82, 70}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{14.0f, 1, 64, 90, 128}, ""),
        new AnimEvent(1.0f + 4 * spb, SetStep, new float[]{spb * 4, 1, 0.2f}, ""),
        new AnimEvent(1.0f + 4 * spb, SetSpeed, new float[]{spb * 6, 3, 0.02f}, ""),
        new AnimEvent(1.0f + 4 * spb, SetColor, new float[]{spb * 6, 1, 1, 0.9f, 0.9f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f + 10 * spb, SetStep, new float[]{0.333f, 1, 0.333f}, ""),
        new AnimEvent(1.0f + 10 * spb, SetRule, new float[]{}, "B2468S1357"),
        new AnimEvent(1.0f + 10 * spb, SetColor, new float[]{1.5f, 1, 0, 0.35f, 0.22f, 0.5f, 1.0f}, ""),
        new AnimEvent(1.0f + 10 * spb, SetColor, new float[]{1.5f, 1, 1, 0.2f, 0.15f, 0.3f, 1.0f}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //scholarsequence
    AnimEvent[] scholarsequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 0.06f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 0.1f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Vanilla"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.85f, 0.7f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.0f, 1.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 121, 170, 75}, ""),

        new AnimEvent(1.0f, EnableSim, new float[]{1}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{8.0f, 1, 121, 80, 75}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //glidersequence
    AnimEvent[] glidersequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, spb/2}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Vanilla"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 1.0f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 1.0f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 6, 9, 16}, ""),

        new AnimEvent(4.0f, EnableSim, new float[]{1}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, ""),
    };
    //lifeinlifesequence
    AnimEvent[] lifeinlifesequence = new AnimEvent[]
    {
        //enable viewport centering
        new AnimEvent(0.0f, SetRule, new float[]{}, "B3S23"),
        new AnimEvent(0.0f, SetSpeed, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetStep, new float[]{-1, 0, 1.0f}, ""),
        new AnimEvent(0.0f, SetColorScheme, new float[]{}, "Lerp"),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 0, 0.7f, 0.7f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 1, 0.7f, 0.7f, 1.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetColor, new float[]{-1, 0, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),
        new AnimEvent(0.0f, SetViewport, new float[]{-1, 0, 1952, 4263, 10}, ""),
        new AnimEvent(0.0f, EnableSim, new float[]{1}, ""),

        new AnimEvent(1.0f, SetStep, new float[]{22.0f, 1, 0.1f}, ""),
        new AnimEvent(1.0f, SetColor, new float[]{11.8f, 1, 0, 1.0f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f, SetColor, new float[]{11.8f, 1, 1, 0.7f, 0.7f, 1.0f, 1.0f}, ""),
        new AnimEvent(1.0f, SetColor, new float[]{11.8f, 1, 2, 0.1f, 0.0f, 0.1f, 1.0f}, ""),
        new AnimEvent(1.0f, SetViewport, new float[]{13.0f, 2, 3820, 5875, 7650}, ""),
        new AnimEvent(4.0f, SetSpeed, new float[]{10.0f, 2, 0.015f}, ""),
        new AnimEvent(14.0f - spb, SetColor, new float[]{spb, 2, 0, 1.0f, 1.0f, 1.0f, 1.0f}, ""),
        new AnimEvent(14.0f - spb, SetColor, new float[]{spb, 2, 1, 0.8f, 0.9f, 0.95f, 1.0f}, ""),
        new AnimEvent(14.0f - spb, SetColor, new float[]{spb, 2, 2, 0.6f, 0.8f, 0.8f, 1.0f}, ""),
        new AnimEvent(14.0f, SetRule, new float[]{}, "B23S34"),
        //S245B368
        new AnimEvent(14.0f, SetColor, new float[]{1.0f, 1, 0, 0.6f, 0.1f, 0.2f, 1.0f}, ""),
        new AnimEvent(14.0f, SetColor, new float[]{3.0f, 1, 1, 0.6f, 0.1f, 0.2f, 1.0f}, ""),
        new AnimEvent(14.0f, SetColor, new float[]{0.3f, 1, 2, 0.0f, 0.0f, 0.0f, 1.0f}, ""),

        new AnimEvent(99999.0f, SetRule, new float[]{}, "Simulation is ended(crashed) here"),
    };

    AnimEvent[] test = new AnimEvent[]
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
