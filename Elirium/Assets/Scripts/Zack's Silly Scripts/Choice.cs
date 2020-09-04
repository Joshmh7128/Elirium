using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    [System.Serializable]
    public class FunctionCalls
    {
        [Tooltip("Game object containing function to be called.")] public GameObject gameObject = null;
        [Tooltip("Name of function to be called. Can't contain parameters.\r\nUse Interact for more dialogue.")] public string methodName = "Interact";
    }

    [Tooltip("The text that will display on the button, representing what the player will say."), TextArea(3, 10)] public string text;

    [Tooltip("List of functions that will be called when the choice is selected.")] public FunctionCalls[] Functions;
}
