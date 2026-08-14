using UnityEngine; // for MonoBehaviour, Collider, AudioClip, AudioSource

// Attach to each collectible clue object in a case scene
public class ClueItem : MonoBehaviour
{
    [SerializeField] private string itemID; // must match one of this case's CaseData.correctItems itemID values
    [SerializeField] private AudioClip pickupSound; // optional, plays on successful collection - drag a sound effect here

    private void OnTriggerEnter(Collider other) // fires when something enters this clue's trigger collider
    {
        if (!other.CompareTag("Player")) // tag-based detection, ignore anything that isn't the player
        {
            return; // exit early for non-player colliders
        }

        if (!CaseHUDController.CluesUnlocked) // guard: clues can't be collected until the player has talked to the spirit
        {
            GameConsoleHUD.Instance?.PrintInteractDesc("Talk to the spirit first."); // gentle hint instead of silently doing nothing
            return; // exit early, do not collect and do not deactivate - the clue stays available for later
        }

        if (pickupSound != null) // guard in case no sound was assigned
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position); // plays even though this object is about to be deactivated
        }

        GameManager.instance.CollectItem(itemID); // also record with GameManager, same as CollectibleItem did, so ComputerScreenManager's success/fail item slots (HasCollectedItem check) still light up correctly
        ClueTracker.Instance.CollectClue(itemID); // report collection to the scene's tracker; CaseHUDController prints the "<item> collected" line and updates the counter
        gameObject.SetActive(false); // hide the clue object so it can't be collected again
    }
}
