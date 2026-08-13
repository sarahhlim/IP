using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the parent door
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the trigger is the Player
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", false);
        }
    }
}