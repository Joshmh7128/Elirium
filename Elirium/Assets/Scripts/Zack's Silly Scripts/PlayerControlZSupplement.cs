using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlZSupplement : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 momentum;
    private Vector3 move;

    private float theta;

    private float interpolation = 1f;

    private void Awake()
    {
        transform.Rotate(Vector3.up * playerTransform.eulerAngles.y * -1);
    }

    public Vector3 Thang(float angle, float x, float z)
    {
        theta = angle * Mathf.Deg2Rad;

        move = (transform.right * (x * Mathf.Cos(theta) + z * Mathf.Sin(theta))) + (transform.forward * (z * Mathf.Cos(theta) + x * Mathf.Cos(theta + Mathf.PI / 2)));

        momentum = Vector3.Lerp(momentum, move, interpolation);
        
        return momentum;
    }
}
