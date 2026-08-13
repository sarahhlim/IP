using UnityEngine; // required for MonoBehaviour and collision types

public class TruckCrashTrigger : MonoBehaviour // fires the crash animation on impact
{
    [SerializeField] private Animator truckAnimator; // reference to the truck's own Animator, assign in inspector

    private void OnCollisionEnter(Collision collision) // called when the truck's collider hits something
    {
        if (collision.gameObject.CompareTag("PMD")) // only react if we hit something tagged PMD
        {
            truckAnimator.SetTrigger("Crash"); // fire the transition into the TruckCrash state
            print("Truck crashed into PMD"); // log the event
        }
    }
}