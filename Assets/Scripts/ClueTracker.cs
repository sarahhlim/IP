using System; // for Action events
using System.Collections.Generic; // for HashSet
using UnityEngine; // for MonoBehaviour, Sprite

// Tracks clue collection progress for the currently loaded case scene, driven by this case's CaseData asset
public class ClueTracker : MonoBehaviour
{
    public static ClueTracker Instance; // scene-local singleton, not persistent across scenes

    [SerializeField] private CaseData caseData; // single source of truth for which items count as clues, how many are required, and their icons

    private HashSet<string> collectedItemIDs = new HashSet<string>(); // prevents double-counting the same clue if triggered twice

    public static event Action<int, int> OnClueCountChanged; // fired whenever a clue is collected, passes (current, total)
    public static event Action<string, Sprite> OnItemCollected; // fired whenever a clue is collected, passes (itemID, icon) for the HUD list
    public static event Action OnAllCluesCollected; // fired once when the required count is reached

    private void Awake()
    {
        Instance = this; // assign singleton reference for this scene only, no DontDestroyOnLoad since each case scene has its own tracker
        collectedItemIDs.Clear(); // ensure a clean state every time the case scene loads
    }

    public void CollectClue(string itemID) // called by ClueItem when the player picks up a clue, icon is looked up from CaseData
    {
        if (collectedItemIDs.Contains(itemID)) // guard against duplicate collection
        {
            print("Clue already collected: " + itemID); // log duplicate attempt
            return; // exit early, do not double count
        }

        CorrectItem matchingItem = FindCorrectItem(itemID); // look up this item's data in the CaseData asset

        if (matchingItem == null) // guard against an itemID that doesn't match any entry in CaseData
        {
            print("WARNING: collected itemID '" + itemID + "' has no matching entry in CaseData.correctItems"); // flag the data mismatch so it gets caught during testing
            return; // exit early, don't count an unrecognized item
        }

        collectedItemIDs.Add(itemID); // mark this clue as collected
        print("Clue collected: " + itemID + " (" + collectedItemIDs.Count + "/" + caseData.correctItems.Length + ")"); // log progress

        OnClueCountChanged?.Invoke(collectedItemIDs.Count, caseData.correctItems.Length); // notify HUD to update the [x/3] counter
        OnItemCollected?.Invoke(itemID, matchingItem.icon); // notify HUD to append an entry to the top-right item list, icon sourced from CaseData

        if (collectedItemIDs.Count >= caseData.correctItems.Length) // check if this collection completes the case
        {
            print("All clues collected for this case"); // log completion
            OnAllCluesCollected?.Invoke(); // notify HUD/return zone that the case objective is complete
        }
    }

    private CorrectItem FindCorrectItem(string itemID) // searches CaseData.correctItems for a matching itemID
    {
        foreach (CorrectItem item in caseData.correctItems) // iterate the case's defined correct items
        {
            if (item.itemID == itemID) // match found
            {
                return item; // return the matching entry
            }
        }

        return null; // no match found
    }

    public bool IsComplete() // exposed for other scripts (e.g. ReturnZoneTrigger) to check completion state
    {
        return collectedItemIDs.Count >= caseData.correctItems.Length; // true once required clue count is reached
    }
}