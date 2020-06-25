using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPuzzleScript : PuzzlePartScript
{
    /// <summary>
    /// The animator attached to this puzzlePart
    /// </summary>
    [HideInInspector] public Animator animator;
    /// <summary>
    /// The list of booleans in the animator, used to trigger animations
    /// </summary>
    [HideInInspector] public List<string> animBoolList;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        // Adds the animation bools to the list for internal use
        animBoolList.Add("white");
        animBoolList.Add("red");
        animBoolList.Add("green");
        animBoolList.Add("blue");
        animBoolList.Add("purple");
    }

    protected override void Update()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activateColor">The color of the signal entering the puzzlePart.</param>
    /// <param name="isActivated">The boolean value of the signal. True = signal starts, False = signal stops.</param>
    /// <param name="source">The GameObject sending the signal.</param>
    public override void Activate(int activateColor, bool isActivated, GameObject source)
    {
        if (isActivated) // Using animation system and ball entering pedestal
        {
            // Passes the information to the animator attached to this puzzlePart
            animator.SetBool(animBoolList[activateColor], true);
        }
        else if (!isActivated) // Using animation system and ball leaving pedestal
        {
            // Passes teh information to the animator attached to this puzzlePart
            animator.SetBool(animBoolList[activateColor], false);
        }
    }
}
