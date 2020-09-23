using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_PuzzleScript : PuzzlePartScript
{
    #region Internal Variables
    [HideInInspector, Tooltip("The point at which the orb is held. Offset in-editor.")] public Transform holdPoint;
    [HideInInspector, Tooltip("The Rigidbody component attached to this GameObject.")] public Rigidbody orbRigidBody;
    [HideInInspector, Tooltip("The start position of this orb, used (tentatively) in the checkpoint system.")] public Vector3 startPos;
    [HideInInspector, Tooltip("Describes whether or not this orb is being held.")] public bool isHeld;
    #endregion

    /// <summary>
    /// Sets default values for variables.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        transform.parent = null;
        orbRigidBody = gameObject.GetComponent<Rigidbody>();

        //define where the object is going to go if it's held
        //holdPoint = GameObject.Find("Hold Point Player").GetComponent<Transform>();

        //set our starting position to the very first position recorded by the game
        startPos = transform.position;
    }

    /// <summary>
    /// Moves the ball with the player's holdPoint location.
    /// </summary>
    public void FixedUpdate()
    {
        if (isHeld)
        {
            Vector3 direction = holdPoint.position - orbRigidBody.position;
            orbRigidBody.velocity = (direction * 20f);
        }
    }

    /// <summary>
    /// Receives a signal, changing the orb's color. This can be done with the light system if the orb is moved off of the LightBlocker tag.
    /// </summary>
    /// <param name="activateColor">The color of the signal entering the puzzlePart.</param>
    /// <param name="isActivated">The boolean value of the signal. True = signal starts, False = signal stops.</param>
    /// <param name="source">The GameObject sending the signal.</param>
    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (isActivated)
        {
            SetColor(activateColor);
        }
    }

    /// <summary>
    /// Resets the position of this orb. Used in the Checkpoint system.
    /// </summary>
    public void ResetPos(Vector3 pos)
    {
        transform.position = pos;
        orbRigidBody.velocity = new Vector3(0, 0, 0);
    }
}
