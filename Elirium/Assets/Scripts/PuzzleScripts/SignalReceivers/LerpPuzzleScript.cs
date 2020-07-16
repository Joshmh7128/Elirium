using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class LerpPuzzleScript : PuzzlePartScript
{
    #region Inspector Variables
    /// <summary>
    /// Distinguishes between speed-based and duration-based lerping, for puzzles which require tight coordination bewteen elements
    /// </summary>
    public enum lerpMode { Speed, Duration }
    /// <summary>
    /// Distinguishes between speed-based and duration-based lerping, for puzzles which require tight coordination between elements
    /// </summary>
    [Header("Lerp Variables")]
    public lerpMode lerpStyle;
    /// <summary>
    /// The delay between the ball going in the pedestal and the puzzlePart starting to move, controllable from the editor
    /// </summary>
    [Range(0, 5)] public float lerpLagTime = 1;
    /// <summary>
    /// The speed at which this puzzlePart moves, controllable from the editor
    /// </summary>
    [ConditionalField("lerpStyle", false, lerpMode.Speed)] [Range(0, 25)] public float lerpSpeed = 5;
    /// <summary>
    /// The speed at which this puzzlePart moves, controllable from the editor
    /// </summary>
    [ConditionalField("lerpStyle", false, lerpMode.Duration)] [Range(0, 25)] public float lerpDuration = 1;
    /// <summary>
    /// If true, the puzzlePart will only move one way. This allows for gates that open and stay open behind the player as they move forwards
    /// </summary>
    public bool oneWay = false;
    /// <summary>
    /// The list of vector3 positions this puzzlePart can lerp to
    /// </summary>
    [HideInInspector] public List<Vector3> lerpActions;
    /// <summary>
    /// One of the vector3 positions that this puzzlePart can lerp to
    /// </summary> // This one is separate to allow the header in-editor
    [Header("Lerp Positions")] public Vector3 whiteLerpAction = new Vector3(-999, -999, -999);
    /// <summary>
    /// One of the vector3 positions that this puzzlePart can lerp to
    /// </summary>
    public Vector3 redLerpAction = new Vector3(-999, -999, -999), greenLerpAction = new Vector3(-999, -999, -999), blueLerpAction = new Vector3(-999, -999, -999), purpleLerpAction = new Vector3(-999, -999, -999);
    #endregion

    #region Internal Variables
    /// <summary>
    /// The time that the lerp started, used in lerp movement
    /// </summary>
    private float lerpStartTime;
    /// <summary>
    /// The initial length of the lerp, used in lerp movement
    /// </summary>
    private float lerpLength;
    /// <summary>
    /// The orb color (in int form) that is currently in the pedestal
    /// </summary>
    private int colorExternal;
    /// <summary>
    /// The orb color (in int form) that is the current target
    /// </summary>
    private int colorInternal;
    /// <summary>
    /// The current target position, used in lerp movement
    /// </summary>
    private Vector3 target;

    private Vector3 checkTarget;
    /// <summary>
    /// The position that this puzzlePart started in, the default that it reverts to
    /// </summary>
    private Vector3 startPos;
    /// <summary>
    /// Is this puzzlePart currently lerping to a target?
    /// </summary>
    private bool isLerping;

    private Vector3 lerpStartPos;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Add the lerp actions to the list for internal use
        lerpActions.Add(whiteLerpAction);
        lerpActions.Add(redLerpAction);
        lerpActions.Add(greenLerpAction);
        lerpActions.Add(blueLerpAction);
        lerpActions.Add(purpleLerpAction);

        // Sets the default values for lerping variables, makes sure the puzzlePart doesn't move immediately
        lerpStartTime = Time.time;
        lerpLength = 0;
        target = transform.position;
        colorExternal = 0; // Indicates that there is nothing in the pedestal
        isLerping = false;

        // Sets the lerpDefaultPos to the current position
        startPos = transform.position;
    }

    /// <summary>
    /// The main function of this script, takes the input from the pedestal and activates the appropriate action
    /// </summary>
    /// <param name="activateColor">The color of the signal entering the puzzlePart.</param>
    /// <param name="isActivated">The boolean value of the signal. True = signal starts, False = signal stops.</param>
    /// <param name="source">The GameObject sending the signal.</param>
    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (isActivated == true && lerpActions[activateColor] != new Vector3(-999, -999, -999)) // Orb is in pedestal and this object should do something
        {
            // Updates the external (current) orb in the pedestal
            colorExternal = activateColor;
            SetColor(activateColor);
            if (transform.position == startPos) // If in the default position
            {
                // Updates the internal (movement target) orb color
                colorInternal = activateColor;
            }
        }
        else // No orb is in the pedestal or the current orb has no associated action
        {
            colorExternal = 0; // Indicates that there is nothing in the pedestal
            SetColor(0);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, target) > .1) // If the target is not ~= the current position
        {
            // Updates distTraveled and lerpFraction based on the time, speed, and distance variables, used in the lerp
            float distTraveled = (Time.time - lerpStartTime) * lerpSpeed;
            float lerpFraction = distTraveled / lerpLength;

            // Changes the position through a lerp
            transform.position = Vector3.Lerp(lerpStartPos, target, lerpFraction);
            // Disables new movement until this lerp is complete
            isLerping = true;

        }
        else if (Vector3.Distance(transform.position, checkTarget) < .1 && isLerping) // If the target is ~= the current position and the puzzlePart was lerping previously
        {
            // Enables new movmement
            isLerping = false;
        }
        else if (Vector3.Distance(transform.position, startPos) < .1 && colorExternal != 0 && lerpActions[colorExternal] != new Vector3(-999, -999, -999) && !isLerping) // At start, but something is in the pedestal
        {
            // Shift the external color (currently in the pedestal) to the internal (current target)
            colorInternal = colorExternal;

            // Sets the lerp variables to begin movement

            lerpLength = Vector3.Distance(transform.position, lerpActions[colorExternal]);
            if (lerpStyle == lerpMode.Duration)
            {
                lerpSpeed = lerpLength / lerpDuration;
            }
            lerpStartPos = transform.position;
            checkTarget = lerpActions[colorInternal]; // Target determined by current orbColor
            StartCoroutine(lerpMove(lerpActions[colorInternal]));

            // Disables new movement until the lerp is complete
            isLerping = true;
        }
        else if (Vector3.Distance(transform.position, target) < .1 && Vector3.Distance(transform.position, startPos) > .1 && colorExternal != colorInternal && !isLerping && !oneWay) // At a position, but that color is not in the pedestal
        {
            // Sets the lerp variables to begin movement
            lerpLength = Vector3.Distance(transform.position, startPos);
            if (lerpStyle == lerpMode.Duration)
            {
                lerpSpeed = lerpLength / lerpDuration;
            }
            lerpStartPos = transform.position;
            checkTarget = startPos; // Target is intial default position
            StartCoroutine(lerpMove(startPos));

            // Disables new movment until the lerp is complete
            isLerping = true;
        }
        else
        {
            // No need to move
        }
    }

    /// <summary>
    /// The coroutine used when an orb enters or leaves the pedestal in lerp mode
    /// </summary>
    /// <param name="OrbColor"></param>
    /// <param name="inPedestal"></param>
    /// <param name="lagTime"></param>
    /// <returns></returns>
    IEnumerator lerpMove(Vector3 targetPos)
    {
        yield return new WaitForSeconds(lerpLagTime);

        lerpStartTime = Time.time;
        target = targetPos;
    }
}
