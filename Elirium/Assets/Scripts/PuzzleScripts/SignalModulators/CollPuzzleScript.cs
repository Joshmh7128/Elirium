using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollPuzzleScript : PuzzlePartScript
{
    [Header("Collector Variables")]
    [Tooltip("Does the order that the sources activate in matter?")] public bool inOrder = false;
    [Tooltip("The sources that activate this puzzlePart")] public List<GameObject> sources;
    [HideInInspector] public List<GameObject> usedSources;
    [HideInInspector] public List<GameObject> colorSources;
    [Tooltip("The list of puzzle objects that this Collector activates")] public List<PuzzlePartScript> puzzleParts;
    /// <summary>
    /// The number of times this Collector has been activated, used to keep track of all of the sources
    /// </summary>
    [HideInInspector] public int numActivations;

    public bool changeColor;

    protected override void Start()
    {
        puzzlePartRend = gameObject.GetComponent<MeshRenderer>();
        usedSources = new List<GameObject>();
        numActivations = 0;
        
        if (changeColor)
        {
            targetMat = new Material(puzzlePartMats[0]);
            targetMat.SetColor("_Color", new Color(0,0,0));
        }
        else
        {
            targetMat = puzzlePartRend.material;
        }
    }

    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (sources.Contains(source))
        {
            if (inOrder && isActivated)
            {
                if (sources[numActivations] == source && !usedSources.Contains(source))
                {
                    numActivations++;
                    usedSources.Add(source);
                    colorSources.Add(source);
                    if (changeColor && source.GetComponent<PuzzlePartScript>().PuzzlePartColorInt != -1) // If the collector changes colors, add this color to its set
                    {
                        SetColor(0);
                    }
                    if (source.gameObject.GetComponent<CondPuzzleScript>()) // If the source is a conduit, set its color
                    {
                        source.gameObject.GetComponent<CondPuzzleScript>().SetColor(activateColor);
                    }
                    if (numActivations == sources.Count)
                    {
                        numActivations = 0;
                        usedSources = new List<GameObject>();
                        colorSources = new List<GameObject>();
                        foreach (PuzzlePartScript activated in puzzleParts)
                        {
                            activated.Activate(activateColor, isActivated, gameObject);
                        }
                    }
                }
                else
                {
                    numActivations = 0;
                    usedSources = new List<GameObject>();
                    colorSources = new List<GameObject>();
                }
            }
            else if (isActivated && !usedSources.Contains(source))
            {
                numActivations++;
                usedSources.Add(source);
                colorSources.Add(source);
                if (source.GetComponent<CondPuzzleScript>())
                {
                    source.gameObject.GetComponent<CondPuzzleScript>().SetColor(activateColor);
                }
                if (changeColor) // If the collector changes colors, add this color to its set
                {
                    SetColor(0);
                }
                if (numActivations == sources.Count)
                {
                    numActivations = 0;
                    usedSources = new List<GameObject>();
                    colorSources = new List<GameObject>();
                    if (changeColor)
                    {
                        SetColor(0);
                    }
                    foreach (PuzzlePartScript activated in puzzleParts)
                    {
                        activated.Activate(activateColor, isActivated, gameObject);
                    }

                }
            }
            else if (!isActivated && changeColor && colorSources.Contains(source))
            {
                colorSources.Remove(source);
            }
            StartCoroutine(resetColors(activateColor, isActivated));
        }
    }

    public IEnumerator resetColors(int activateColor, bool isActivated)
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject cond in sources)
        {
            if (cond.GetComponent<CondPuzzleScript>() && !usedSources.Contains(cond))
            {
                cond.GetComponent<CondPuzzleScript>().SetColor(0);
            }
        }
    }

    public override void SetColor(int colorNum)
    {
        targetMat.color = puzzlePartRend.material.color;
        if (changeColor)
        {
            targetMat.color = new Color(0, 0, 0);
            targetMat.SetColor("_EmissionColor", new Color(0, 0, 0));

            float colorFrac = (float)numActivations / (float)sources.Count;

            targetMat.color = new Color(colorFrac, colorFrac, colorFrac);
            targetMat.SetColor("_EmissionColor", targetMat.color);
        }
    }
}
