using UnityEngine; // needed for MonoBehaviour, Collider

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemID; // set to match a correctItem's ID, or leave as a decoy ID that matches nothing

    void OnTriggerEnter(Collider other) // tag-based, matches your existing pattern
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.CollectItem(itemID); // always records the pickup, correct or decoy
            Destroy(gameObject); // remove from the scene once collected
        }
    }
}