using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCavesGondola : MonoBehaviour
{
    public GameObject Player;
    public Animator anim;

    private void Start()
    {
        Player = GameObject.Find("PlayerZ");
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            anim.SetTrigger("PlayerEnter");
        }
    }
}
