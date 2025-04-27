using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthSystem))]
public abstract class BotBase : MonoBehaviour
{
    public enum State { Wander, Chase, Attack }
    public State currentState;

    [Header("Movement")]
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public float stuckCheckInterval = 1f;
    public float stuckMoveDistance = 0.5f;

    [Header("Attack")]
    public float attackCooldown = 1f;

    protected HealthSystem healthSystem;
    protected NavMeshAgent agent;
    protected Transform target;
    protected float lastAttackTime;

    Vector3 lastPosition;
    float stuckTimer;

    protected virtual void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void Update()
    {
        if (healthSystem.IsDead())
        {
            agent.isStopped = true;
            return;
        }

        FindOrUpdateTarget();

        if (target == null)
        {
            currentState = State.Wander;
            Wander();
        }
        else
        {
            float distance = Vector2.Distance(transform.position, target.position);
            currentState = (distance <= attackRange) ? State.Attack : State.Chase;
        }

        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                TryAttack();
                break;
        }

        CheckIfStuck();
    }

    protected virtual void FindOrUpdateTarget()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= detectionRadius)
            return;

        HealthSystem[] allUnits = FindObjectsOfType<HealthSystem>();
        Transform closest = null;
        float closestDistance = detectionRadius;

        foreach (var unit in allUnits)
        {
            if (unit == healthSystem || unit.IsDead() || unit.myType != healthSystem.targetType)
                continue;

            float dist = Vector2.Distance(transform.position, unit.transform.position);
            if (dist < closestDistance)
            {
                closest = unit.transform;
                closestDistance = dist;
            }
        }

        target = closest;
    }

    protected virtual void Wander()
    {
        if (agent.hasPath) return;

        Vector2 randomPoint = Random.insideUnitCircle * (detectionRadius * 0.5f);
        Vector3 destination = (Vector2)transform.position + randomPoint;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    protected virtual void Chase()
    {
        if (target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    protected virtual void TryAttack()
    {
        agent.isStopped = true;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            AttackLogic();
        }
    }

    protected abstract void AttackLogic();

    // Антизастревание
    void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckCheckInterval)
        {
            float moved = Vector2.Distance(transform.position, lastPosition);
            if (moved < 0.1f && !agent.pathPending)
            {
                ForceMoveAway();
            }
            lastPosition = transform.position;
            stuckTimer = 0f;
        }
    }

    void ForceMoveAway()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 destination = (Vector2)transform.position + randomDirection * stuckMoveDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    protected virtual void OnTargetKilled()
    {
        target = null;
        currentState = State.Wander;
        agent.isStopped = false;
    }

}
