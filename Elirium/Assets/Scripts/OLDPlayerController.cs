using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[AddComponentMenu("PlayerController")]

public class OLDPlayerController : MonoBehaviour
{
    #region Settings

    #region LookSettings
    [Header("Look Settings", order = 0)]
    [Space(10, order = 1)]

    [Tooltip("How fast the mouse moves the camera")] [Range(0.1f, 3)] public float mouseSensitivity = 1.5f;
    [Tooltip("The camera attached to the player")] public Transform playerCamera;
    [Tooltip("The crosshair sprite")] public Sprite Crosshair;
    internal Vector3 cameraStartingPosition;

    [HideInInspector] public Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;
    [Space(15, order = 2)]
    #endregion

    #region MoveSettings
    [Header("Movement Settings", order = 3)]
    [Space(10, order = 4)]

    public bool playerCanMove = true;
    [Tooltip("How fast the player walks")] [Range(0.1f, 10)] public float walkSpeed = 4f;
    [Tooltip("How fast the player sprints")] [Range(0.1f, 10)] public float sprintSpeed = 9f;
    [Tooltip("How high the player jumps")] [Range(0.1f, 15)] public float jumpPower = 9f;

    [HideInInspector] public float speed;
    internal float walkSpeedInternal;
    internal float sprintSpeedInternal;
    internal float jumpPowerInternal;

    [System.Serializable]
    public class CrouchModifiers
    {
        [Tooltip("Input Axis for crouching, set up in the InputManager")] public string crouchInputAxis;
        [Tooltip("Multiplier for the player's walk speed while crouching")] [Range(0.01f, 1.5f)] public float crouchWalkSpeedModifier = 0.5f;
        [Tooltip("Multiplier for the player's sprint speed while crouching")] [Range(0.01f, 1.5f)] public float crouchSprintSpeedModifier = 0.25f;
        [Tooltip("Multiplier for the player's jump height while crouching")] [Range(0.01f, 1.5f)] public float crouchJumpPowerModifier = 0f;
        [Tooltip("Toggle this to override the crouch input axis from another script")] public bool crouchOverride;

        internal float colliderHeight;
    }
    public CrouchModifiers _crouchModifiers = new CrouchModifiers();

    [System.Serializable]
    public class AdvancedSettings
    {
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
        [Range(0, 89)] public float maxSlopeAngle = 70;
        [HideInInspector] public bool tooSteep;
        [HideInInspector] public RaycastHit surfaceAngleCheck;
    }
    public AdvancedSettings advanced = new AdvancedSettings();
    private CapsuleCollider capsule;
    private const float jumpRayLength = 0.7f;
    public bool isGrounded { get; private set; }
    Vector2 inputXY;
    [HideInInspector] public bool isCrouching;
    bool isSprinting = false;

    [HideInInspector] public Rigidbody fps_RigidBody;
    #endregion

    #endregion

    private void Awake()
    {
        #region Look Settings - Awake
        originalRotation = transform.localRotation.eulerAngles;
        #endregion

        #region Movement Settings - Awake
        walkSpeedInternal = walkSpeed;
        sprintSpeedInternal = sprintSpeed;
        jumpPowerInternal = jumpPower;
        capsule = GetComponent<CapsuleCollider>();
        isGrounded = true;
        isCrouching = false;
        fps_RigidBody = GetComponent<Rigidbody>();
        _crouchModifiers.colliderHeight = capsule.height;
        #endregion

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        #region Look Settings - Start
        //Places the crosshair in the middle of the screen
        GameObject qui = new GameObject("AutoCrosshair");
        qui.AddComponent<RectTransform>();
        qui.AddComponent<Canvas>();/*
        qui.AddComponent<CanvasScaler>();
        qui.AddComponent<GraphicRaycaster>();
        qui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GameObject quic = new GameObject("Crosshair");
        quic.AddComponent<Image>().sprite = Crosshair;
        qui.transform.localScale *= 2;*/

        qui.transform.SetParent(this.transform);
        qui.transform.position = Vector3.zero;/*
        quic.transform.SetParent(qui.transform);
        quic.transform.position = Vector3.zero;*/

        cameraStartingPosition = playerCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        #endregion
    }

    private void Update()
    {
        #region Look Settings - Update
        float mouseXInput;
        float mouseYInput;
        mouseXInput = Input.GetAxis("Mouse Y");
        mouseYInput = Input.GetAxis("Mouse X");
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
        #region Movement Settings - FixedUpdate

        bool wasWalking = !isSprinting;
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        advanced.tooSteep = false;
        float inrSprintSpeed;
        inrSprintSpeed = sprintSpeedInternal;
        Vector3 dMove = Vector3.zero;
        speed = true ? isCrouching ? walkSpeedInternal : (isSprinting ? inrSprintSpeed : walkSpeedInternal) : (isSprinting ? walkSpeedInternal : inrSprintSpeed);
        Ray ray = new Ray(transform.position, -transform.up);
        if (isGrounded || fps_RigidBody.velocity.y < 0.1)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, capsule.height * jumpRayLength);
            float nearest = float.PositiveInfinity;
            isGrounded = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].collider.isTrigger && hits[i].distance < nearest)
                {
                    isGrounded = true;
                    nearest = hits[i].distance;
                }
            }
        }

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z + 0.1f), Vector3.down, out advanced.surfaceAngleCheck, 1f))
        {

            if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) < 89)
            {
                advanced.tooSteep = false;
                dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal;
                if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > advanced.maxSlopeAngle)
                {
                    advanced.tooSteep = true;
                    isSprinting = false;
                    dMove = new Vector3(0, -4, 0);

                }
                else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > 44)
                {
                    advanced.tooSteep = true;
                    isSprinting = false;
                    dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x) + new Vector3(0, -4, 0);
                }
            }
        }
        else if (Physics.Raycast(new Vector3(transform.position.x - 0.086f, transform.position.y - 0.75f, transform.position.z - 0.05f), Vector3.down, out advanced.surfaceAngleCheck, 1f))
        {

            if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) < 89)
            {
                advanced.tooSteep = false;
                dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal;
                if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > 70)
                {
                    advanced.tooSteep = true;
                    isSprinting = false;
                    dMove = new Vector3(0, -4, 0);

                }
                else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > 45)
                {
                    advanced.tooSteep = true;
                    isSprinting = false;
                    dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x) + new Vector3(0, -4, 0);

                }
            }
            else if (Physics.Raycast(new Vector3(transform.position.x + 0.086f, transform.position.y - 0.75f, transform.position.z - 0.05f), Vector3.down, out advanced.surfaceAngleCheck, 1f))
            {

                if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) < 89)
                {
                    advanced.tooSteep = false;
                    dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal;
                    if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > 70)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        dMove = new Vector3(0, -4, 0);

                    }
                    else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up) > 45)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x) + new Vector3(0, -4, 0);
                    }
                }
            }
        }
        else
        {
            advanced.tooSteep = false;
            dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal;
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputXY = new Vector2(horizontalInput, verticalInput);
        if (inputXY.magnitude > 1) { inputXY.Normalize(); }

        float yv = fps_RigidBody.velocity.y;
        bool didJump = true ? Input.GetButton("Jump") : Input.GetButtonDown("Jump");

        if (isGrounded && didJump && jumpPowerInternal > 0)
        {
            yv += jumpPowerInternal;
            isGrounded = false;
            didJump = false;
        }

        if (playerCanMove)
        {
            fps_RigidBody.velocity = dMove + Vector3.up * yv;
        }
        else
        {
            fps_RigidBody.velocity = Vector3.zero;
        }

        if (dMove.magnitude > 0 || !isGrounded || advanced.tooSteep){
            GetComponent<Collider>().material = advanced.zeroFrictionMaterial;
        }

        isCrouching = _crouchModifiers.crouchOverride ? true : Input.GetAxis(_crouchModifiers.crouchInputAxis) > 0;

        if (isCrouching)
        {
            capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight / 2, 5 * Time.deltaTime);
            walkSpeedInternal = walkSpeed * _crouchModifiers.crouchWalkSpeedModifier;
            sprintSpeedInternal = sprintSpeed * _crouchModifiers.crouchSprintSpeedModifier;
            jumpPowerInternal = jumpPower * _crouchModifiers.crouchJumpPowerModifier;
        }
        else
        {
            capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight, 5 * Time.deltaTime);
            walkSpeedInternal = walkSpeed;
            sprintSpeedInternal = sprintSpeed;
            jumpPowerInternal = jumpPower;
        }
        #endregion
    }

    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
        fps_RigidBody.velocity = new Vector3(0, 0, 0);
    }
}
