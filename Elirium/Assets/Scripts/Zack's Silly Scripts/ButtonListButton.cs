using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    public TextMeshProUGUI myText;
    public ButtonListControl buttonControl;

    public Choice.FunctionCalls[] functions;

    public void SetText(string textString)
    {
        myText.text = textString;
    }

    public void Thing()
    {
        foreach (Choice.FunctionCalls thing in functions)
        {
            thing.gameObject.SendMessage(thing.methodName);
        }
    }
}
