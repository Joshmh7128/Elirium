using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControlZ : MonoBehaviour
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
    [Tooltip("The jumping height for the player"), Range(1, 5)] public float jumpHeight = 4f;
    [Tooltip("The maximum vertical velocity for the player"), Range(-25, -100)] public int terminalVelocity = -50;

    public Transform groundCheck;
    public LayerMask groundMask;

    public Transform ceilingCheck;

    public Transform compass;
    public PlayerControlZSupplement supplement;

    [SerializeField]private float gravity = -37.5f;

    /// <summary>
    /// Container class for crouching options. Contains separate jump height, sprint speed, and others.
    /// </summary>
    [System.Serializable]
    public class crouchOptions
    {
        [Tooltip("The input axis that the player uses to crouch")] public string crouchInputAxis = "crouch";
        [Tooltip("The player's height modifier when crouching"), Range(0, 1)] public float crouchSizeModifier = 0.7f;

        [Header("Crouch Movement Settings")]
        [Tooltip("The player's walking speed when crouching"), Range(1, 25)] public int crouchWalkSpeed = 5;
        [Tooltip("The player's sprinting speed when crouching"), Range(1, 25)] public int crouchSprintSpeed = 5;
        [Tooltip("The player's jump height when crouching"), Range(1, 5)] public float crouchJumpHeight = 1f;
    }
    [Tooltip("Container for options related to crouching")] public crouchOptions crouching;
    #endregion

    #region Internal Variables
    private CharacterController controller;
    
    /// <summary>
    /// The current speed that the player's movement is multiplied by. Changes if the player is crouching, used when walking
    /// </summary>
    private int speedInternal;
    /// <summary>
    /// The current sprinting speed for the player. Changes when the player is crouching, used when sprinting
    /// </summary>
    private int sprintSpeedInternal;
    /// <summary>
    /// The current jumping height for the player. Changes when the player is crouching
    /// </summary>
    private float jumpHeightInternal;
    /// <summary>
    /// The current movement vector, determined by the x and y inputs from the keyboard/controller.
    /// </summary>
    private Vector3 movement;

    private float mouseXInput;
    private float mouseYInput;

    private float xInput;
    private float yInput;

    private bool isGrounded;
    private bool wasGrounded;

    private float stepOffsetInternal;
    #endregion

    #endregion

    [Space(15, order = 5)]

    #region LookVariables
    [Header("Look Setting", order = 6)]
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
        jumpHeightInternal = jumpHeight;

        controller = GetComponent<CharacterController>();

        stepOffsetInternal = controller.stepOffset;

        wasGrounded = false;
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        rewiredInput = ReInput.players.SystemPlayer;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        #region Look Update
        mouseXInput = rewiredInput.GetAxis(verticalLookAxis);
        mouseYInput = rewiredInput.GetAxis(horizontalLookAxis);

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
        compass.transform.rotation = Quaternion.Euler(0, 0, 0);
        #endregion

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.49f, groundMask);

        if (!isGrounded)
        {
            controller.stepOffset = 0;
            if (wasGrounded && movement.y < 0)
            {
                movement.y = 0;
            }
        }
        else
        {
            if (movement.y < 0)
            {
                controller.stepOffset = stepOffsetInternal;
            }
        }

        wasGrounded = isGrounded;

        if (Physics.CheckSphere(ceilingCheck.position, 0.05f, groundMask) && movement.y > 0)
        {
            movement.y = 0;
        }
    }
    
    private void FixedUpdate()
    {
        GetMovementInput();
        ProcessMovementInput();
    }

    private void GetMovementInput()
    {
        xInput = rewiredInput.GetAxis(horizontalMovementAxis);
        yInput = rewiredInput.GetAxis(verticalMovementAxis);

        movement.x = 0;
        movement.z = 0;
        movement += supplement.Thang(followAngles.y, xInput, yInput);

        if (rewiredInput.GetAxis(sprintAxis) != 0)
        {
            movement.x *= sprintSpeedInternal;
            movement.z *= sprintSpeedInternal;
        }
        else
        {
            movement.x *= speedInternal;
            movement.z *= speedInternal;
        }

        if (rewiredInput.GetAxis(jumpAxis) != 0 && isGrounded && movement.y < 0)
        {
            movement.y = Mathf.Sqrt(jumpHeightInternal * -2f * gravity);
        }
    }

    private void ProcessMovementInput()
    {
        movement.y += gravity / 50;
        movement.y = Mathf.Max(movement.y, terminalVelocity);
        controller.Move(movement / 50);
    }
    
    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    // this script pushes all rigidbodies that the character touches
    float pushPower = 2.0f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
