using UnityEngine; // for MonoBehaviour, Collider

// Attach to each collectible clue object in a case scene
public class ClueItem : MonoBehaviour
{
    [SerializeField] private string itemID; // must match one of this case's CaseData.correctItems itemID values

    private void OnTriggerEnter(Collider other) // fires when something enters this clue's trigger collider
    {
        if (!other.CompareTag("Player")) // tag-based detection, ignore anything that isn't the player
        {
            return; // exit early for non-player colliders
        }

        ClueTracker.Instance.CollectClue(itemID); // report collection to the scene's tracker; CaseHUDController prints the "<item> collected" line and updates the counter
        gameObject.SetActive(false); // hide the clue object so it can't be collected again
    }
}