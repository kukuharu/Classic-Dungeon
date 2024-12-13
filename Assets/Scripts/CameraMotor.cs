using System.Collections;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    public float boundX = 0.15f;
    public float boundY = 0.05f;

    private Vector3 originalPosition; // Store the original position for proper reset
    private Vector3 shakeOffset = Vector3.zero;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
        originalPosition = transform.position; // Save initial position
    }

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        // This is to check if we're inside the bounds on the x axis
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }

        // This is to check if we're inside the bounds on the y axis
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        // Apply screen shake offset
        if (shakeDuration > 0)
        {
            shakeOffset = (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        // Update the camera position with shake and reset to original
        Vector3 targetPosition = originalPosition + new Vector3(delta.x, delta.y, 0) + shakeOffset;
        transform.position = targetPosition;

        // Update original position for following the player
        originalPosition = transform.position - shakeOffset;

        // Test screen shake by pressing F
        //if (Input.GetKeyDown(KeyCode.F))
        //{
            //TriggerScreenShake(0.3f, 0.01f); // Adjust test values as needed
        //}
    }

    public void TriggerScreenShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
