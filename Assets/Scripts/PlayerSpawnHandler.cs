using UnityEngine; // needed for MonoBehaviour
using UnityEngine.SceneManagement; // needed for SceneManager, sceneLoaded event

public class PlayerSpawnHandler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (spawnPoint != null)
        {
            CharacterController cc = GetComponentInChildren<CharacterController>(); // finds it on PlayerCapsule

            if (cc != null)
            {
                cc.enabled = false; // disable so it can't resist the position change

                cc.transform.position = spawnPoint.transform.position; // move THIS object (PlayerCapsule), not the root
                cc.transform.rotation = spawnPoint.transform.rotation;

                cc.enabled = true; // re-enable after teleport
            }
            else
            {
                print("No CharacterController found in children - cannot reposition player correctly");
            }
        }
        else
        {
            print("No SpawnPoint found in scene: " + scene.name);
        }
    }
}