using UnityEngine;

public class MonsterLoading : MonoBehaviour
{
    public bool IsLoading { get; private set; } = true;
    public float loadingDuration = 2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isLoading", true);
        }
        Invoke(nameof(EndLoading), loadingDuration);
    }

    private void EndLoading()
    {
        IsLoading = false;
        if (animator != null)
        {
            animator.SetBool("isLoading", false);
        }
    }
}