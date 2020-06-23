using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    #region Movement Variables
    [Header("Movement Settings", order = 0)]
    [Space(10, order = 1)]
    #region External Variables
    [Tooltip("The default (walking) speed for the player"), Range(1, 25)] public int defaultSpeed = 5;
    [Tooltip("The sprinting speed for the player"), Range(1, 25)] public int sprintSpeed = 7;
    [Tooltip("The crouching speed for the player"), Range(1, 25)] public int crouchSpeed = 3;
    [Tooltip("The jumping force for the player"), Range(1,25)] public int jumpForce = 5;
    #endregion
    #region Internal Variables
    /// <summary>
    /// The current speed that the player's movement is multiplied by. Changes if the player is walking, sprinting, or crouching.
    /// </summary>
    private int currentSpeed;
    /// <summary>
    /// The current movement vector, determined by the x and y inputs from the keyboard/controller.
    /// </summary>
    private Vector3 movement;
    #endregion
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
