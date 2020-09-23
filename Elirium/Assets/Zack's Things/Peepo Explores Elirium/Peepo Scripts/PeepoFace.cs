using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeepoFace : MonoBehaviour
{
    public MeshRenderer leftEye;
    public MeshRenderer rightEye;

    public Material closed;
    public Material open;

    private void Start()
    {
        StartCoroutine(Eyes());
    }

    private IEnumerator Eyes()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(.5f, 2f));

            leftEye.material = closed;
            rightEye.material = closed;

            yield return new WaitForSeconds(.15f);

            leftEye.material = open;
            rightEye.material = open;
        }
    }
}
