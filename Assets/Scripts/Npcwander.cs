using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("Assign your pavement plane here")]
    public Transform pavementPlane;

    [Header("Wander Settings")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 4f;

    private NavMeshAgent agent;
    private Bounds pavementBounds;
    private float waitTimer;
    private bool waiting;
    private bool boundsValid;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (pavementPlane == null)
        {
            Debug.LogError($"[{name}] Pavement Plane not assigned!", this);
            return;
        }

        // Search the plane AND all its children for renderers,
        // then combine all their bounds into one.
        Renderer[] renderers = pavementPlane.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError($"[{name}] No Renderer found on '{pavementPlane.name}' or its children.", this);
            return;
        }

        pavementBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            pavementBounds.Encapsulate(renderers[i].bounds);
        }

        boundsValid = true;
        PickNewDestination();
    }

    void Update()
    {
        if (!boundsValid) return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                PickNewDestination();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
    }

    void PickNewDestination()
    {
        // Pick a random point within the combined bounds' XZ area
        float x = Random.Range(pavementBounds.min.x, pavementBounds.max.x);
        float z = Random.Range(pavementBounds.min.z, pavementBounds.max.z);
        Vector3 randomPoint = new Vector3(x, pavementBounds.center.y, z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            waiting = true;
            waitTimer = 0.1f;
        }
    }
}