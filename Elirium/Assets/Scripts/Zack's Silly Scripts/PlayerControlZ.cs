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
    [Tooltip("Can the player move?"), SerializeField] private bool playerCanMove = true;
    [Tooltip("The default (walking) speed for the player"), Range(1, 25)] public int defaultSpeed = 5;
    [Tooltip("The sprinting speed for the player"), Range(1, 25)] public int sprintSpeed = 7;
    [Tooltip("The jumping height for the player"), Range(1, 5)] public float jumpHeight = 4f;
    [Tooltip("The maximum vertical velocity for the player"), Range(-25, -100)] public int terminalVelocity = -50;
    [Tooltip("The location used to determine if the player is grounded.")] public Transform groundCheck;
    [Tooltip("The layers of objects that the controller will recognize as ground objects.")] public LayerMask groundMask;
    [Tooltip("The location used to determine if the player has hit the ceiling.")] public Transform ceilingCheck;
    [Tooltip("Affects the rate at which the player accelerates downward."), Range(-10, -100)] public float gravity = -37.5f;
    [Tooltip("Affects the rate at which the player's velocity is changed due to player input. Lower values means momentum is conserved more.")] public float interpolationFactor;
    [Tooltip("Affects the speed that the player will slide down walls."), Range(0, 1)] public float slideFriction;
    
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
    /// A variable the script uses to enable/disable player movement.
    /// </summary>
    [HideInInspector] public bool playerCanMoveInternal;

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
    /// The direction of the player's desired movement, determined by the x and y inputs from the keyboard/controller.
    /// </summary>
    private Vector3 move;
    /// <summary>
    /// Similar to the previous Vector, except this is lerped to create smoother movement.
    /// </summary>
    private Vector3 horizontalMomentum;
    /// <summary>
    /// The player's current velocity (under most circumstances). The y value will be incorrect while grounded, otherwise this is the vector used to move the character.
    /// </summary>
    private Vector3 movement;
    /// <summary>
    /// Used to modify the previous vector in certain circumstances (such as sliding on slopes) while keeping those values intact.
    /// </summary>
    private Vector3 finalMovement;
    /// <summary>
    /// Variables that hold the inputs from the mouse, used to look around.
    /// </summary>
    private float mouseXInput;
    private float mouseYInput;
    /// <summary>
    /// Variables that hold inputs for the player to move.
    /// </summary>
    private float xInput;
    private float yInput;
    /// <summary>
    /// Booleans that return true if the player is or was grounded. Can be used in tandem to determine the frame the player leaves or lands on the ground.
    /// </summary>
    [HideInInspector] public bool isGrounded;
    private bool wasGrounded;
    /// <summary>
    /// Holds information from raycasts and spherecasts shot at the ground. Used to determine the angle of the ground the player is standing on.
    /// </summary>
    private RaycastHit groundHit;
    /// <summary>
    /// Booleans that return true if the player is or was sliding.
    /// </summary>
    private bool isSliding;
    private bool wasSliding;
    /// <summary>
    /// Holds the value of the stepOffset from the character controller. The controller's value is changed, so this is used to "remember" the original value.
    /// </summary>
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

    /// <summary>
    /// Variable that is true if the game is paused. If true, this script basically stops working until it is false.
    /// </summary>
    [HideInInspector] public bool gameIsPaused = false;
    #endregion


    private void Awake()
    {
        gameIsPaused = false;

        #region Look Awake
        originalRotation = transform.localRotation.eulerAngles;
        #endregion

        #region Movement Awake
        playerCanMoveInternal = true;

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, 0.49f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!gameIsPaused)
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

            if (playerCanMove && playerCanMoveInternal)
            {
                targetAngles.y += mouseYInput * mouseSensitivity;
                targetAngles.x += mouseXInput * mouseSensitivity;
            }
            targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * Mathf.Infinity, 0.5f * Mathf.Infinity);
            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * 170, 0.5f * 170);
            if (!playerCanMoveInternal)
            {
                followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, 0.2f);
            }
            else
            {
                followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, 0.05f);
            }
            playerCamera.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
            transform.rotation = Quaternion.Euler(0f, followAngles.y + originalRotation.y, 0);
            #endregion

            #region Move Update
            isGrounded = Physics.SphereCast(groundCheck.position + Vector3.up, 0.49f, Vector3.down, out groundHit, 1, groundMask);
            
            Physics.Raycast(groundHit.point + new Vector3(0, .1f, 0), Vector3.down, out groundHit, 0.15f, groundMask);
            isSliding = false;
            if (isGrounded && movement.y <= 0)
            {
                isSliding = Vector3.Angle(Vector3.up, groundHit.normal) > controller.slopeLimit;
            }

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

            /*if (Physics.CheckSphere(ceilingCheck.position, 0.05f, groundMask) && movement.y > 0)
            {
                movement.y = 0;
            }*/
            ProcessMovementInput();
            #endregion
        }
    }

    private void ProcessMovementInput()
    {
        if (wasSliding && !isSliding && movement.y <= 0)
        {
            movement = finalMovement;
            horizontalMomentum = new Vector3(finalMovement.x, 0, finalMovement.z) / speedInternal;
        }

        if (playerCanMove && playerCanMoveInternal)
        {
            xInput = rewiredInput.GetAxis(horizontalMovementAxis);
            yInput = rewiredInput.GetAxis(verticalMovementAxis);
        }
        else
        {
            xInput = 0;
            yInput = 0;
        }
        
        move = (new Vector3(1, 0, 0) * (xInput * Mathf.Cos(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad) + yInput * Mathf.Sin(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad))) + (new Vector3(0, 0, 1) * (yInput * Mathf.Cos(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad) + xInput * Mathf.Cos(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad + Mathf.PI / 2)));

        horizontalMomentum = Vector3.Lerp(horizontalMomentum, move, interpolationFactor * Time.deltaTime);

        if (rewiredInput.GetAxis(sprintAxis) != 0)
        {
            movement.x = horizontalMomentum.x * sprintSpeedInternal;
            movement.z = horizontalMomentum.z * sprintSpeedInternal;
        }
        else
        {
            movement.x = horizontalMomentum.x * speedInternal;
            movement.z = horizontalMomentum.z * speedInternal;
        }
        
        if (rewiredInput.GetAxis(jumpAxis) > 0 && isGrounded && movement.y < 0 && !isSliding && playerCanMoveInternal)
        {
            movement.y = Mathf.Sqrt(jumpHeightInternal * -2f * gravity);
        }

        movement.y += gravity * Time.deltaTime;
        movement.y = Mathf.Max(movement.y, terminalVelocity);

        if (isSliding)
        {
            Vector3 slopeDirection = Quaternion.Euler(0, -90, 0) * Vector3.Normalize(new Vector3(groundHit.normal.x, 0, groundHit.normal.z));
            Vector3 horizontalStuff = new Vector3(movement.x, 0, movement.z);
            finalMovement = Vector3.Normalize(Vector3.RotateTowards(horizontalStuff, slopeDirection, Mathf.PI, 0)) * Vector3.Magnitude(horizontalStuff) * Mathf.Cos(Vector3.Angle(horizontalStuff, slopeDirection) * Mathf.Deg2Rad);
            
            finalMovement += Vector3.RotateTowards(groundHit.normal, Vector3.down, Mathf.PI / 2, 0) * -1 * slideFriction * movement.y;
        }
        else
        {
            finalMovement = movement;
        }

        controller.Move(finalMovement * Time.deltaTime);

        wasSliding = isSliding;
    }
    
    public void ResetPos(Vector3 pos)
    {
        gameIsPaused = true;
        transform.position = pos;
        gameIsPaused = false;
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
