<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListControl : MonoBehaviour
{
    public GameObject buttonTemplate;

    private List<GameObject> buttons = new List<GameObject>();

    private GameObject button;

    private ButtonListButton buttonStuff;

    public void GenerateList(Choice[] choices)
    {
        // Clear the list of previous buttons.
        if (buttons.Count > 0)
        {
            foreach (GameObject button in buttons)
            {
                Destroy(button.gameObject);
            }

            buttons.Clear();
        }

        // Spawn in the buttons.
        foreach (Choice choice in choices)
        {
            button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            buttons.Add(button);

            buttonStuff = button.GetComponent<ButtonListButton>();

            buttonStuff.SetText(choice.text);
            buttonStuff.functions = choice.Functions;

            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListControl : MonoBehaviour
{
    public GameObject buttonTemplate;

    private List<GameObject> buttons = new List<GameObject>();

    private GameObject button;

    private ButtonListButton buttonStuff;

    public void GenerateList(Choice[] choices)
    {
        // Clear the list of previous buttons.
        if (buttons.Count > 0)
        {
            foreach (GameObject button in buttons)
            {
                Destroy(button.gameObject);
            }

            buttons.Clear();
        }

        // Spawn in the buttons.
        foreach (Choice choice in choices)
        {
            button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            buttons.Add(button);

            buttonStuff = button.GetComponent<ButtonListButton>();

            buttonStuff.SetText(choice.text);
            buttonStuff.functions = choice.Functions;

            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }
}
>>>>>>> zack
