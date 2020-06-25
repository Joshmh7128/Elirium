using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondPuzzleScript : PuzzlePartScript
{
    [Header("Conduit Variables")]
    public bool collecting;
    [Tooltip("The list of puzzle objects that this Conduit activates")] public List<PuzzlePartScript> puzzleParts;

    private List<int> activatedColors = new List<int>();

    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (puzzlePartMats.Count > 0)
        {
            if (isActivated)
            {
                ActivateList(activateColor, isActivated, source);
                if (!activatedColors.Contains(activateColor))
                {
                    activatedColors.Add(activateColor);
                }
            }
            else if (!collecting)
            {
                ActivateList(activateColor, false, source);
                if (activatedColors.Contains(activateColor))
                {
                    activatedColors.Remove(activateColor);
                    if (activatedColors.Count > 0)
                    {
                        ActivateList(activatedColors[0], true, source);
                    }
                }

            }
        }

    }

    public void ActivateList(int activateColor, bool isActivated, GameObject source)
    {
        foreach (PuzzlePartScript activated in puzzleParts)
        {
            activated.Activate(activateColor, isActivated, gameObject);
        }
        if (isActivated)
        {
            SetColor(activateColor);
        }
        else
        {
            SetColor(0);
        }
    }
}
