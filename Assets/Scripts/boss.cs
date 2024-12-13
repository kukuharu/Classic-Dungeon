using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MovingEnemy
{
    public float[] orbitalProjectileSpeed = { 2.5f, -2.5f };
    public float[] distance = { 0.25f, 0.4f };
    public List<Transform> orbitals;  // Changed to List for easier management

    private void Update()
    {
        // Loop through the orbitals and check if each one is still alive
        for (int i = orbitals.Count - 1; i >= 0; i--)
        {
            if (orbitals[i] == null)
            {
                // Remove null (destroyed) orbitals from the list
                orbitals.RemoveAt(i);
            }
            else
            {
                // Move the orbital around the boss
                orbitals[i].position = transform.position +
                    new Vector3(-Mathf.Cos(Time.time * orbitalProjectileSpeed[i]) * distance[i],
                                Mathf.Sin(Time.time * orbitalProjectileSpeed[i]) * distance[i],
                                0);
            }
        }
    }
}