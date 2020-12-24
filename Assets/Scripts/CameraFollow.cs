using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference: Brackeys @ YouTube

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position;
        // Account for offset using target's local axes
        desiredPosition += ( offset.x * target.transform.right );
        desiredPosition += ( offset.y * target.transform.up );
        desiredPosition += ( offset.z * target.transform.forward );

        Vector3 smoothedPosition = Vector3.Lerp( transform.position, desiredPosition, smoothSpeed );
        transform.position = smoothedPosition;
    }
}
