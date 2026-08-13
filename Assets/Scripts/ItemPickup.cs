using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Phone";
    public bool isRequiredItem = true; // Set to FALSE for decoy/false items!

    [Header("Audio")]
    public AudioClip pickupSound;

    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the player entered the item's trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        // If player is close enough and presses 'E'
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        // Play pickup audio clip
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Send collection update to manager if present
        ItemCollectorManager.Instance?.ItemCollected(itemName, isRequiredItem);

        // Hide/Remove the item from the ground
        gameObject.SetActive(false);
    }
}