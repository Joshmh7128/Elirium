using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    #region Input Axes
    [Tooltip("The input axis for horizontal (left/right) movement")] public string horizontalMovementAxis = "moveHorizontal";
    [Tooltip("The input axis for vertical (front/back) movement")] public string verticalMovementAxis = "moveVertical";
    [Tooltip("The input axis for horizontal camera control")] public string horizontalLookAxis = "lookHorizontal";
    [Tooltip("The input axis for vertical camera control")] public string verticalLookAxis = "lookVertical";
    #endregion

    #region Movement Variables
    [Header("Movement Settings", order = 0)]
    [Space(10, order = 1)]
    #region External Variables
    [Tooltip("Can the player move?")] public bool playerCanMove;
    [Tooltip("The default (walking) speed for the player"), Range(1, 25)] public int defaultSpeed = 5;
    [Tooltip("The sprinting speed for the player"), Range(1, 25)] public int sprintSpeed = 7;
    [Tooltip("The jumping force for the player"), Range(1, 25)] public int jumpForce = 5;

    /// <summary>
    /// Container class for crouching options. Contains separate jump force, walk speed, sprint speed, and others.
    /// </summary>
    [System.Serializable]
    public class crouchOptions
    {
        [Tooltip("The input axis that the player uses to crouch")] public string crouchInputAxis;
        [Tooltip("The player's height modifier when crouching"), Range(0, 1)] public float crouchSizeModifier = 0.7f;

        [Header("Crouch Movement Settings")]
        [Tooltip("The player's walking speed when crouching"), Range(1, 25)] public int crouchWalkSpeed = 5;
        [Tooltip("The player's sprinting speed when crouching"), Range(1, 25)] public int crouchSprintSpeed = 5;
        [Tooltip("The player's jump force when crouching"), Range(1, 25)] public int crouchJumpForce = 5;
    }
    [Tooltip("Container for options related to crouching")] public crouchOptions crouching;
    #endregion

    #region Internal Variables
    /// <summary>
    /// The current speed that the player's movement is multiplied by. Changes if the player is crouching, used when walking
    /// </summary>
    private int speedInternal;
    /// <summary>
    /// The current sprinting speed for the player. Changes when the player is crouching, used when sprinting
    /// </summary>
    private int sprintSpeedInternal;
    /// <summary>
    /// The current jumping force for the player. Changes when the player is crouching
    /// </summary>
    private int jumpForceInternal;
    /// <summary>
    /// The current movement vector, determined by the x and y inputs from the keyboard/controller.
    /// </summary>
    private Vector3 movement;
    #endregion

    #endregion

    #region LookVariables

    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
    }
}
