using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [System.Serializable]
    public class FunctionCalls
    {
        [Tooltip("Game object containing function to be called.")] public GameObject gameObject = null;
        [Tooltip("Name of function to be called. Can't contain parameters.\r\nUse Interact for more dialogue.\r\nUse StartChoices to let the player respond.")] public string methodName = "Interact";
    }

    [Tooltip("Name of the NPC.")] public string name;

    [Tooltip("Affects the rate at which dialogue text appears.")] public float typeSpeed = 15f;
    
    [Tooltip("The position that the player cam will rotate towards.")] public Transform lookPoint;

    [Tooltip("The lines that the NPC will say."), TextArea(3, 10)] public string[] sentences;
    
    [Tooltip("List of functions that will be called when the text is finished.")] public FunctionCalls[] Functions;
}
