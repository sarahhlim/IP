using UnityEngine; // for MonoBehaviour, Collider
using UnityEngine.InputSystem; // for Keyboard, New Input System

// Attach to an empty GameObject with a BoxCollider (Is Trigger checked) placed around the spirit NPC.
// When the player walks into range and presses E, plays the spirit's reaction line in the console panel
// and kicks off the clue-finding phase (clue counter + timer) via CaseHUDController.
public class SpiritInteraction : MonoBehaviour
{
    private bool playerInRange; // true while the player is inside this trigger
    private bool hasTriggered; // true once the dialogue has fired, prevents re-triggering on repeat visits

    private void Awake()
    {
        Collider col = GetComponent<Collider>(); // grab whatever collider is on this object

        if (col != null && !col.isTrigger) // sanity check, this script only works with a trigger collider
        {
            Debug.LogWarning("SpiritInteraction on " + gameObject.name + " requires its Collider to have Is Trigger enabled."); // flag misconfiguration early
        }
    }

    private void OnTriggerEnter(Collider other) // fires when something enters this trigger
    {
        if (other.CompareTag("Player")) // tag-based detection, matches the rest of the project's interaction pattern
        {
            playerInRange = true; // allow the E prompt to fire in Update
        }
    }

    private void OnTriggerExit(Collider other) // fires when something leaves this trigger
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // stop listening for E once the player walks away
        }
    }

    private void Update() // checks for the interact key press each frame while the player is in range
    {
        if (playerInRange && !hasTriggered && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            hasTriggered = true; // one-time trigger, talking to the spirit again won't replay the dialogue or restart the timer
            CaseHUDController.Instance?.TriggerSpiritDialogue(); // prints the reaction line, pins the clue counter, and starts the timer
        }
    }
}
