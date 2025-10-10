using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CatNavNPC : MonoBehaviour
{
    [Header("Wandering")]
    public float wanderRadius = 5f;
    public float idleTimeMin = 2f;
    public float idleTimeMax = 5f;

    [Header("Animator")]
    public Animator animator;
    public string speedParam = "Vert";  // blend parameter for walking
    public string stateParam = "State"; // 0 = idle, 1 = walking
    public float animationSpeed = 1f;   // constant animation speed

    [Header("Movement")]
    public float moveSpeed = 1.5f;      // NavMeshAgent speed
    public float rotationSpeed = 120f;  // degrees per second

    private NavMeshAgent agent;
    private Vector3 startPosition;
    private float idleTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;

        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.updateRotation = false; // we handle rotation manually

        PickNewTarget();
    }

    void Update()
    {
        agent.speed = moveSpeed;

        if (agent.pathPending)
            return;

        // If agent can't reach target, pick a new one
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            PickNewTarget();
            return;
        }

        // Check if arrived at destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
                PickNewTarget();
        }

        // Smoothly rotate toward movement direction
        RotateTowardsMovement();

        UpdateAnimator();
    }

    void PickNewTarget()
    {
        bool found = false;

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 point = startPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                found = true;
                break;
            }
        }

        if (!found)
        {
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
            return;
        }

        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
    }

    void RotateTowardsMovement()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimator()
    {
        bool isMoving = agent.velocity.magnitude > 0.01f;

        if (isMoving)
        {
            animator.SetFloat(speedParam, 1f);
            if (!string.IsNullOrEmpty(stateParam))
                animator.SetFloat(stateParam, 1f);
        }
        else
        {
            animator.SetFloat(speedParam, 0f);
            if (!string.IsNullOrEmpty(stateParam))
                animator.SetFloat(stateParam, 0f);
        }

        animator.speed = animationSpeed;
    }
}



