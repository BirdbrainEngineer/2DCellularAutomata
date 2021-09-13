using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject dispatcherObject;
    private Dispatcher dispatcher;

    public string rule;

    private int[] rules = new int[9];

    private bool stateChanged = false;

    void Start()
    {
        dispatcher = dispatcherObject.GetComponent<Dispatcher>();
    }

    void Update(){
        if(stateChanged){
            
        }
    }

    //Converts string based rule into internal "rules". Returns whether rules have changed  
    private bool ParseRules(){ 
        int[] newRules = {0, 0, 0, 0, 0, 0, 0, 0, 0};
        bool born = false;
        bool survive = false;
        bool validRules = true;
        bool rulesChanged = false;

        foreach(char character in rule){
            if(character == 'B' || character == 'b'){
                born = true;
                survive = false;
                continue;
            }
            if(character == 'S' ||character == 's'){
                born = false;
                survive = true;
                continue;
            }
        //rules meaning (indexed by number of neighbor cells alive):
        //0: alive = dead   ||  dead = dead
        //1: alive = dead   ||  dead = alive
        //2: alive = alive  ||  dead = dead
        //3: alive = alive  ||  dead = alive
            int num = (int)char.GetNumericValue(character);
            if(num < 0 || num > 8){
                print("Malformed rules!");
                validRules = false;
                break;
            }
            else {
                if(born){ newRules[num] += 1; }
                else if(survive){ newRules[num] += 2; }
                else{
                    print("Malformed rules!");
                    validRules = false;
                    break;
                }
            }
        }
        if(validRules){
            for(int i = rules.Length - 1; i >= 0; i--){
                if(newRules[i] < 0 || newRules[i] > 3){
                    rulesChanged = false;
                    print("Malformed rules!");
                    break;
                }
                if(rules[i] != newRules[i]){
                    rulesChanged = true;
                    break;
                }
            }
            if(rulesChanged){
                rules = newRules;
                stateChanged = true;
            }
        }
        return rulesChanged;
    }
}
