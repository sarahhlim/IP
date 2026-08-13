using UnityEngine;

public class ItemCollectorManager : MonoBehaviour
{
    public static ItemCollectorManager Instance;

    [Header("Progress")]
    public int itemsCollected = 0;
    public int totalRequiredItems = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ItemCollected(string itemName, bool isRequired)
    {
        if (isRequired)
        {
            itemsCollected++;
            Debug.Log($"Collected required item: {itemName} ({itemsCollected}/{totalRequiredItems})");

            if (itemsCollected >= totalRequiredItems)
            {
                Debug.Log("🎉 All 3 items collected! Talk to the ghost to finish.");
            }
        }
        else
        {
            Debug.Log($"Collected decoy item: {itemName}. This wasn't related to the accident!");
        }
    }
}