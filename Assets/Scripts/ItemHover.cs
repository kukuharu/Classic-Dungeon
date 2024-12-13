using UnityEngine;

public class ItemHover : MonoBehaviour
{
    public float amplitude = 0.015f; // Height of the hover effect
    public float frequency = 2f; // Speed of the hover effect

    private Vector3 posOrigin; // Original position for the hover
    private Vector3 tempPos; // Temporary position for hover calculations

    private void Awake()
    {
        // Initialize the hover origin to the current position
        posOrigin = transform.position;
    }

    private void OnEnable()
    {
        // Ensure hover starts when the object is enabled
        posOrigin = transform.position;
    }

    private void Update()
    {
        // Apply hover effect
        tempPos = posOrigin;
        tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }

    /// <summary>
    /// Adjusts the hover origin dynamically (optional).
    /// </summary>
    public void SetHoverOrigin(Vector3 newOrigin)
    {
        posOrigin = newOrigin;
    }
}
