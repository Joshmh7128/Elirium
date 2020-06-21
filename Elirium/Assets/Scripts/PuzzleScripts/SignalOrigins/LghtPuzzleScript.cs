using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LghtPuzzleScript : PuzzlePartScript
{
    protected LineRenderer lineRend;

    RaycastHit lightHit;
    [Header("Light variables")]
    protected bool isHittingLens = false;
    [Range(1,500)]public float rayLength = 15;
    private PuzzlePartScript currentHit;

    private GameObject lastHit;
    private LensPuzzleScript lastLensHit = null;

    protected override void Start()
    {
        lineRend = gameObject.GetComponent<LineRenderer>();

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        #region HitsLens
        if (Physics.Raycast(transform.position, transform.forward, out lightHit, rayLength) && lightHit.transform.gameObject.GetComponent<LensPuzzleScript>()) // If the ray hits a lens
        {
            LensPuzzleScript hitLens = lightHit.transform.gameObject.GetComponent<LensPuzzleScript>();
            lastLensHit = hitLens;
            lineRend.SetPosition(1, transform.InverseTransformPoint(lightHit.point));
            Vector3 lensHitPoint = lightHit.point;

            if (Physics.Raycast(transform.position, transform.forward, out lightHit, rayLength, ~LayerMask.GetMask("Lenses")) && lastHit == null) // If the ray hits something and hit nothing on the last frame
            {
                PuzzlePartScript hitScript = lightHit.transform.gameObject.GetComponent<PuzzlePartScript>();
                if (hitScript != null && lightHit.transform.tag != "LightBlocker")
                {
                    hitScript.Activate(hitLens.LensActivate(true, lensHitPoint, lightHit.point), true, gameObject);
                    currentHit = hitScript;
                }
                lastHit = lightHit.transform.gameObject;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out lightHit, rayLength, ~LayerMask.GetMask("Lenses")) && lastHit != null) // If the ray hits something and hit something on the last frame
            {
                if (lightHit.transform.gameObject != lastHit)
                {
                    if (lastHit.GetComponent<PuzzlePartScript>() && lastHit.tag != "LightBlocker")
                    {
                        lastHit.GetComponent<PuzzlePartScript>().Activate(hitLens.LensActivate(true, lensHitPoint, lightHit.point), false, gameObject);
                        currentHit = null;
                    }
                    lastHit = null;
                }
                else
                {
                    hitLens.LensActivate(true, lensHitPoint, lightHit.point);
                }

            }
            else // If the ray hits nothing
            {
                if (lastHit != null)
                {
                    if (lastHit.GetComponent<PuzzlePartScript>() && lastHit.tag != "LightBlocker")
                    {
                        lastHit.GetComponent<PuzzlePartScript>().Activate(hitLens.LensActivate(false, lightHit.point, transform.position + transform.forward * rayLength), false, gameObject);
                        currentHit = null;
                    }
                    lastHit = null;
                }
                hitLens.LensActivate(true, lensHitPoint, transform.position + transform.forward * rayLength);
            }
        }

        #endregion

        #region noLensHit
        else
        {
            if (lastLensHit != null)
            {
                lastLensHit.LensActivate(false, Vector3.zero, Vector3.zero);
                lastLensHit = null;
            }
            if (Physics.Raycast(transform.position, transform.forward, out lightHit, rayLength, 1) && lastHit == null) // If the ray hits something and hit nothing on the last frame
            {
                PuzzlePartScript hitScript = lightHit.transform.gameObject.GetComponent<PuzzlePartScript>();
                //Debug.DrawRay(transform.position, transform.forward * lightHit.distance, Color.red);
                if (hitScript != null && lightHit.transform.tag != "LightBlocker")
                {
                    hitScript.Activate(PuzzlePartColorInt, true, gameObject);
                    currentHit = hitScript;
                    lineRend.SetPosition(1, transform.InverseTransformPoint(lightHit.point));
                }
                lastHit = lightHit.transform.gameObject;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out lightHit, rayLength, 1) && lastHit != null) // If the ray hits something and hit something on the last frame
            {
                if (lightHit.transform.gameObject != lastHit)
                {
                    if (lastHit.GetComponent<PuzzlePartScript>() && lastHit.tag != "LightBlocker")
                    {
                        lastHit.GetComponent<PuzzlePartScript>().Activate(PuzzlePartColorInt, false, gameObject);
                        currentHit = null;
                    }
                    lastHit = null;
                }
                Debug.DrawRay(transform.position, transform.forward * lightHit.distance, Color.blue);
                lineRend.SetPosition(1, transform.InverseTransformPoint(lightHit.point));
            }
            else // If the ray hits nothing
            {
                if (lastHit != null)
                {
                    if (lastHit.GetComponent<PuzzlePartScript>() && lastHit.tag != "LightBlocker")
                    {
                        lastHit.GetComponent<PuzzlePartScript>().Activate(PuzzlePartColorInt, false, gameObject);
                        currentHit = null;
                    }
                    lastHit = null;
                }
                Debug.DrawRay(transform.position, transform.forward * rayLength, Color.yellow);
                lineRend.SetPosition(1, transform.InverseTransformPoint(transform.position + transform.forward * rayLength));

            }
        }
        #endregion
    }

    /// <summary>
    /// When activated, the light source will change its own color to the one that it has been activated by. 
    /// </summary>
    /// <param name="activateColor">The color of the signal entering the puzzlePart.</param>
    /// <param name="isActivated">The boolean value of the signal. True = signal starts, False = signal stops.</param>
    /// <param name="source">The GameObject sending the signal.</param>
    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (isActivated)
        {
            SetColor(activateColor);
            if (currentHit != null)
                currentHit.Activate(activateColor, true, gameObject);
        }
        else
        {
            SetColor((int)puzzlePartColor);
            if (currentHit != null)
                currentHit.Activate((int)puzzlePartColor, true, gameObject);
        }
    }

    /// <summary>
    /// Updates the color of this emitter
    /// </summary>
    /// <param name="colorNum">The color to update to</param>
    public override void SetColor(int colorNum)
    {
        base.SetColor(colorNum);

        lineRend.material = puzzlePartMats[colorNum];
    }

    public Vector3 GetHitPosition()
    {
        return lightHit.point;
    }

    public Quaternion GetHitAngle()
    {
        return transform.rotation;
    }
}
