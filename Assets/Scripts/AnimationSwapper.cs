using System.Collections.Generic;
using UnityEngine;

public class AnimationSwapper : MonoBehaviour
{
    public static AnimationSwapper instance;

    public Animator characterAnimator;
    public List<AnimatorOverrideController> animationControllers;

    private void Awake()
    {
        // Ensure only one instance of AnimationSwapper
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwapAnimation(int index)
    {
        if (index >= 0 && index < animationControllers.Count)
        {
            characterAnimator.runtimeAnimatorController = animationControllers[index];
        }
        else
        {
            Debug.LogWarning("Invalid index for animationControllers list.");
        }
    }
}