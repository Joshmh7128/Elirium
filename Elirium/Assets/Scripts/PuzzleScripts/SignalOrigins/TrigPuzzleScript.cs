using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigPuzzleScript : PuzzlePartScript
{
    #region Inspector Variables
    [Header("Trigger Variables")]
    [Tooltip("The orb detection trigger on this pedestal.")] public Collider orbDetect;
    [Tooltip("The list of puzzle objects that this pedestal activates")] public List<PuzzlePartScript> puzzleParts;
    #endregion

    /// <summary>
    /// Detects when an orb enters the trigger, and Activate()'s each of its puzzleParts with that orb's color.
    /// </summary>
    /// <param name="orbColl">The collider of the orb/other object.</param>
    private void OnTriggerEnter(Collider orbColl)
    {
        if (orbColl.GetComponent<Orb_PuzzleScript>())
        {
            //Debug.Log("Orb in");
            foreach (PuzzlePartScript activated in puzzleParts)
            {
                activated.Activate(orbColl.GetComponent<Orb_PuzzleScript>().PuzzlePartColorInt, true, gameObject);
            }
            SetColor(orbColl.GetComponent<Orb_PuzzleScript>().PuzzlePartColorInt);
        }
        else
        {
            //Debug.Log("Not orb!");
        }
    }

    /// <summary>
    /// Detects when an orb leaves the trigger, and de-Activate()'s each of its puzzleParts with that orb's color.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Orb_PuzzleScript>())
        {
            //Debug.Log("Orb out");
            foreach(PuzzlePartScript activated in puzzleParts)
            {
                activated.Activate(other.GetComponent<Orb_PuzzleScript>().PuzzlePartColorInt, false, gameObject);
            }

            SetColor(0);
        }
    }

    /// <summary>
    /// Updates the color of this puzzlePart using the SetColor() base function.
    /// </summary>
    /// <param name="activateColor">The color of the signal entering the puzzlePart.</param>
    /// <param name="isActivated">The boolean value of the signal. True = signal starts, False = signal stops.</param>
    /// <param name="source">The GameObject sending the signal.</param>
    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (isActivated)
        {
            SetColor(activateColor);
        }
        else
        {
            SetColor((int)puzzlePartColor);
        }

    }
}
