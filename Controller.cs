using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject dispatcherObject;
    private Dispatcher dispatcher;

    public string rule;

    private int[] rules = new int[9];

    void Start()
    {
        dispatcher = dispatcherObject.GetComponent<Dispatcher>();
    }

    void Update()
    {
        
    }
}
