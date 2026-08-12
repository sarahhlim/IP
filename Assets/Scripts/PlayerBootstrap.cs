using UnityEngine; // needed for MonoBehaviour, SerializeField, Instantiate

public class PlayerBootstrap : MonoBehaviour // place on an empty object in the Office scene ONLY
{
    [SerializeField] private GameObject playerPrefab; // drag the NestedParent_Unpack PREFAB ASSET here (from Project window)

    void Awake()
    {
        if (PersistentPlayer.instance == null) // only spawn if no player exists yet
        {
            Instantiate(playerPrefab); // creates it fresh, its own Awake() will DontDestroyOnLoad it
            print("Player spawned by bootstrap");
        }
        else
        {
            print("Player already exists, skipping spawn"); // this is what happens on every return to Office
        }
    }
}