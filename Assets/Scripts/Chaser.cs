using UnityEngine;
using UnityEngine.AI; // Import the namespace for Unity's navigation system, which includes the NavMeshAgent class

public class Chaser : MonoBehaviour
{
    [SerializeField]
    private Transform targetToChase; // Reference to the Transform of the target object that this chaser will follow

    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component that handles pathfinding and movement 

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to this GameObject
    }

    void Update()
    {
        if(navMeshAgent != null && targetToChase != null)
        {
            navMeshAgent.SetDestination(targetToChase.position); // Continuously update the destination of the NavMeshAgent to follow the target's current position
        }
    }
}