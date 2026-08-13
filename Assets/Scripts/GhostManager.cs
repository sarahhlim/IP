using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance;

    [Header("Ghost Chasers")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Vector3 ghostSpawnOffset = new Vector3(-2f, 0f, 0f);

    private List<GameObject> activeGhosts = new List<GameObject>();
    private Transform playerTransform;

    public int GhostCount => activeGhosts.Count;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            print("GhostManager: No object tagged 'Player' found in scene " + scene.name);
            return;
        }

        playerTransform = playerObj.transform;

        // Re-target and reposition every existing ghost to follow the fresh player object
        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost == null) continue;

            Chaser chaser = ghost.GetComponent<Chaser>();
            if (chaser != null)
            {
                chaser.targetToChase = playerTransform;
            }

            ghost.transform.position = playerTransform.position + ghostSpawnOffset;
        }
    }

    // Call this wherever a "fail" happens, e.g. GhostManager.instance.SpawnGhost();
    public void SpawnGhost()
    {
        if (ghostPrefab == null)
        {
            Debug.LogWarning("GhostManager: ghostPrefab not assigned!");
            return;
        }

        Vector3 spawnPos = playerTransform != null
            ? playerTransform.position + ghostSpawnOffset
            : transform.position;

        GameObject newGhost = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
        DontDestroyOnLoad(newGhost);

        Chaser chaser = newGhost.GetComponent<Chaser>();
        if (chaser != null && playerTransform != null)
        {
            chaser.targetToChase = playerTransform;
        }

        activeGhosts.Add(newGhost);
        print("Ghost spawned. Total ghosts: " + activeGhosts.Count);
    }
}