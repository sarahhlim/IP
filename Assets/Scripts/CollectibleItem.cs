using UnityEngine; // needed for MonoBehaviour, Collider, AudioSource

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemID; // set to match a correctItem's ID, or leave as a decoy ID that matches nothing
    [SerializeField] private AudioClip pickupSound; // drag a sound effect here

    void OnTriggerEnter(Collider other) // tag-based, matches your existing pattern
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.CollectItem(itemID); // always records the pickup, correct or decoy

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position); // plays even though this object is about to be destroyed
            }

            Destroy(gameObject); // remove from the scene once collected
        }
    }
}