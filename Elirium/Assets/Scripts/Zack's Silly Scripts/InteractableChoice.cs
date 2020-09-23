<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChoice : MonoBehaviour
{
    public Choice[] choices;

    public void StartChoices()
    {
        FindObjectOfType<DialogueManager>().SetButtons(choices);
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChoice : MonoBehaviour
{
    public Choice[] choices;

    public void StartChoices()
    {
        FindObjectOfType<DialogueManager>().SetButtons(choices);
    }
}
>>>>>>> zack
