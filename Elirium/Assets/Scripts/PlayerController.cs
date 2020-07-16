using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    #region Variables

    private Player rewiredInput;

    #region Input Axes
    [Header("Input Axes", order = 0)]
    [Space(10, order = 1)]
    [Tooltip("The input axis for horizontal (left/right) movement")] public string horizontalMovementAxis = "moveHorizontal";
    [Tooltip("The input axis for vertical (front/back) movement")] public string verticalMovementAxis = "moveVertical";
    [Tooltip("The input axis for horizontal camera control")] public string horizontalLookAxis = "lookHorizontal";
    [Tooltip("The input axis for vertical camera control")] public string verticalLookAxis = "lookVertical";

    [Tooltip("The input button for sprinting")] public string sprintAxis = "sprint";
    [Tooltip("The input button for jumping")] public string jumpAxis = "jump";
    #endregion

    [Space(15, order = 2)]

    #region Movement Variables
    [Header("Movement Settings", order = 3)]
    [Space(10, order = 4)]

    #region External Variables
    [Tooltip("Can the player move?")] public bool playerCanMove = true;
    [Tooltip("The default (walking) speed for the player"), Range(1, 25)] public int defaultSpeed = 5;
    [Tooltip("The sprinting speed for the player"), Range(1, 25)] public int sprintSpeed = 7;
    [Tooltip("The jumping force for the player"), Range(20, 100)] public int jumpForce = 75;

    /// <summary>
    /// Container class for crouching options. Contains separate jump force, walk speed, sprint speed, and others.
    /// </summary>
    [System.Serializable]
    public class crouchOptions
    {
        [Tooltip("The input axis that the player uses to crouch")] public string crouchInputAxis = "crouch";
        [Tooltip("The player's height modifier when crouching"), Range(0, 1)] public float crouchSizeModifier = 0.7f;

        [Header("Crouch Movement Settings")]
        [Tooltip("The player's walking speed when crouching"), Range(1, 25)] public int crouchWalkSpeed = 5;
        [Tooltip("The player's sprinting speed when crouching"), Range(1, 25)] public int crouchSprintSpeed = 5;
        [Tooltip("The player's jump force when crouching"), Range(1, 25)] public int crouchJumpForce = 5;
    }
    [Tooltip("Container for options related to crouching")] public crouchOptions crouching;
    #endregion

    #region Internal Variables
    private CapsuleCollider coll;

    private Rigidbody rb;

    private const float jumpRayLength = 2.25f;

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

    [Space(15, order = 5)]

    #region LookVariables
    [Header("Look Settings", order = 6)]
    [Space(10, order = 7)]

    [Tooltip("How fast the mouse moves the camera"), Range(0.1f, 3)] public float mouseSensitivity = 1.5f;
    [Tooltip("The camera attached to the player")] public Transform playerCamera;

    [HideInInspector] public Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;
    #endregion
    #endregion

    private void Awake()
    {
        #region Look Awake
        originalRotation = transform.localRotation.eulerAngles;
        #endregion

        #region Movement Awake
        speedInternal = defaultSpeed;
        sprintSpeedInternal = sprintSpeed;
        jumpForceInternal = jumpForce;

        coll = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        //rewiredInput = ReInput.players.GetPlayer(0);
        rewiredInput = ReInput.players.SystemPlayer;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region Look Update
        float mouseXInput;
        float mouseYInput;

        mouseXInput = rewiredInput.GetAxis(verticalLookAxis);
        mouseYInput = rewiredInput.GetAxis(horizontalLookAxis);

        //mouseXInput = Input.GetAxis("Mouse Y");
        //mouseYInput = Input.GetAxis("Mouse X");
        if (targetAngles.y > 180)
        {
            targetAngles.y -= 360;
            followAngles.y -= 360;
        }
        else if (targetAngles.y < -180)
        {
            targetAngles.y += 360;
            followAngles.y += 360;
        }
        if (targetAngles.x > 180)
        {
            targetAngles.x -= 360;
            followAngles.x -= 360;
        }
        else if (targetAngles.x < -180)
        {
            targetAngles.x += 360;
            followAngles.x += 360;
        }

        targetAngles.y += mouseYInput * mouseSensitivity;
        targetAngles.x += mouseXInput * mouseSensitivity;
        targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * Mathf.Infinity, 0.5f * Mathf.Infinity);
        targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * 170, 0.5f * 170);
        followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, 0.05f);
        playerCamera.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0f, followAngles.y + originalRotation.y, 0);
        #endregion
    }

    private void FixedUpdate()
    {
        GetMovementInput(); 
        ProcessMovementInput();
    }

    private void GetMovementInput()
    {
        Vector3 xInput = rewiredInput.GetAxis(horizontalMovementAxis) * transform.right;
        Vector3 yInput = rewiredInput.GetAxis(verticalMovementAxis) * transform.forward;

        movement = xInput + yInput;

        if (rewiredInput.GetAxis(sprintAxis) != 0)
        {
            movement *= sprintSpeedInternal;
        }
        else
        {
            movement *= speedInternal;
        }

        Debug.DrawRay(transform.position, -transform.up * jumpRayLength);
        if (rewiredInput.GetAxis(jumpAxis) != 0 && Physics.Raycast(transform.position, -transform.up, jumpRayLength))
        {
            //movement.y = rewiredInput.GetAxis(jumpAxis) * jumpForceInternal;
            rb.AddForce(transform.up * jumpForceInternal * 15, ForceMode.Impulse);
        }
        else
        {
            movement.y = rb.velocity.y - 9.8f * 4 * Time.deltaTime;
        }
    }

    private void ProcessMovementInput()
    {
        rb.velocity  = movement;
    }

    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
    }
}
