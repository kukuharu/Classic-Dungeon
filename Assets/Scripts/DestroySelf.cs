using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float effectDuration = 0.5f; // Adjust this based on your animation length

    void Start()
    {
        Destroy(gameObject, effectDuration);
    }
}

// This script is for FX on instantiated Game Objects