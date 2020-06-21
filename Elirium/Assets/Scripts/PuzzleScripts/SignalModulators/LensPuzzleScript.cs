using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensPuzzleScript : PuzzlePartScript
{
    protected LineRenderer lineRend;
    public List<Material> lineRendMats;

    protected override void Start()
    {
        lineRend = gameObject.GetComponent<LineRenderer>();

        base.Start();
    }

    /// <summary>
    /// Updates the color of this emitter
    /// </summary>
    /// <param name="colorNum">The color to update to</param>
    public override void SetColor(int colorNum)
    {
        base.SetColor(colorNum);

        lineRend.material = lineRendMats[colorNum];
    }

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

    public int LensActivate(bool isActivated, Vector3 hitPosition, Vector3 lineEndPosition)
    {
        if (isActivated)
        {
            lineRend.SetPosition(0, transform.InverseTransformPoint(hitPosition));
            lineRend.SetPosition(1, transform.InverseTransformPoint(lineEndPosition));
            if (lineRend.enabled == false)
            {
                lineRend.enabled = true;
            }
        }
        else
        {
            lineRend.enabled = false;
        }
        return PuzzlePartColorInt;
    }
}
