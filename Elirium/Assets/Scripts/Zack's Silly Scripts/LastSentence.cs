using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastSentence : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator.SetBool("IsOpen", false);
    }

    public void Animate()
    {
        if (animator.GetBool("IsOpen"))
        {
            animator.SetBool("IsOpen", false);
        }
        else
        {
            animator.SetBool("IsOpen", true);
        }
    }
}
