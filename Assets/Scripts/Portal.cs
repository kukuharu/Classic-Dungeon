using UnityEngine;

public class Portal : Collidable
{
    public string sceneName; // Assign this in the Inspector
    private bool isTransitioning = false; // Prevent multiple triggers

    protected override void OnCollide(Collider2D coll)
    {
        if (!isTransitioning && coll.name == "Player")
        {
            isTransitioning = true; // Block further triggers
            GameManager.instance.SaveState(); // Save progress
            SceneController.instance.ChangeScene(sceneName); // Trigger transition
        }
    }
}