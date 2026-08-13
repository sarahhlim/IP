using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class ItemPickup : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Phone";
    public bool isRequiredItem = true; // Set to FALSE for decoy items

    [Header("Audio")]
    public AudioClip pickupSound;

    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
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
        // New Input System check for pressing the 'E' key
        if (isPlayerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        ItemCollectorManager.Instance?.ItemCollected(itemName, isRequiredItem);

        gameObject.SetActive(false);
    }
}