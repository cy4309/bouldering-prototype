using UnityEngine;

public class ClimberAnim : MonoBehaviour
{
    Animator animator;
    HoldContact contact;

    void Awake()
    {
        contact = GetComponent<HoldContact>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (animator == null || contact == null)
        {
            return;
        }

        animator.SetBool("Climbing", contact.IsClimbing || contact.IsHanging);
    }
}