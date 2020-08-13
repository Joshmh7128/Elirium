﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogue dialogue;

    public void Interact()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}