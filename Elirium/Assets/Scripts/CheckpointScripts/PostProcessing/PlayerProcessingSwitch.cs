using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerProcessingSwitch : MonoBehaviour
{
    public PostProcessProfile loadProfile;
    public GameObject player;

    private void Start()
    {
        player = FindObjectOfType<CameraScript>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PostProcessVolume>().profile = loadProfile;
        }
    }
}
