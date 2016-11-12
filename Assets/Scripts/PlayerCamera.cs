using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    Transform target;
    public float horizontalSpeed = 3.0f;
    public float verticalSpeed = 1.0f;

    private float lookRotDecelleration = 0.96f;

    Vector3 lookRotVelocity = Vector3.zero;
    Vector3 displacement = Vector3.zero;

    void Start()
    {
        target = transform.parent.GetComponentInChildren<PlayerController>().transform;
        displacement = target.position - transform.position;
    }

    public void RotatePivot(Vector3 lookRotation) {
        lookRotVelocity *= lookRotDecelleration;

        lookRotVelocity += lookRotation * 0.1f;
        if (lookRotVelocity.x > 1) lookRotVelocity.x = 1;
        if (lookRotVelocity.x < -1) lookRotVelocity.x = -1;
        if (lookRotVelocity.y > 1) lookRotVelocity.y = 1;
        if (lookRotVelocity.y < -1) lookRotVelocity.y = -1;

        transform.RotateAround(target.position, Vector3.up, lookRotVelocity.x * horizontalSpeed);
        transform.RotateAround(target.position, Vector3.right, -lookRotVelocity.y * verticalSpeed);

        //transform.position = target.position
        transform.LookAt(target.position + Vector3.up * 5);
    }
}
