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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Get the plane's world-space bounds (works for a standard Unity Plane or a scaled Quad)
        Renderer rend = pavementPlane.GetComponent<Renderer>();
        pavementBounds = rend.bounds;

        PickNewDestination();
    }

    void Update()
    {
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
        // Pick a random point within the plane's XZ bounds
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
