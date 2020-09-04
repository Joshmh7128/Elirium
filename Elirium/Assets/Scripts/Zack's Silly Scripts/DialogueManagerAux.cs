using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManagerAux : MonoBehaviour
{
    private PlayerControlZ player;
    private Camera playerCam;

    private void Awake()
    {
        player = FindObjectOfType<PlayerControlZ>();
        playerCam = FindObjectOfType<Camera>();
    }

    public void DoThing(float delay, Transform lookPoint)
    {
        StartCoroutine(MoveCamera(delay, lookPoint));
    }

    #region Some Vectors
    Vector3 tempVector;
    Vector3 tempVectorHor;
    Vector3 tempForward;
    Vector3 tempVectorVert;
    #endregion

    private IEnumerator MoveCamera(float delay, Transform lookPoint)
    {
        yield return new WaitForSeconds(0.1f);

        while (!player.isGrounded)
        {
            yield return null;
        }

        yield return new WaitForSeconds(delay - 0.1f);

        tempVector = Vector3.Normalize(lookPoint.position - playerCam.transform.position);
        tempVectorHor = Vector3.Normalize(new Vector3(tempVector.x, 0, tempVector.z));
        tempForward = playerCam.transform.TransformDirection(Vector3.forward);
        tempVectorVert = tempForward;
        tempVectorVert.y = tempVector.y;

        player.targetAngles.y += Vector3.SignedAngle(Vector3.Normalize(new Vector3(tempForward.x, 0, tempForward.z)), tempVectorHor, Vector3.up);
        player.targetAngles.x -= Vector3.SignedAngle(Vector3.Normalize(tempForward), tempVectorVert, playerCam.transform.TransformDirection(Vector3.right));
    }
}
