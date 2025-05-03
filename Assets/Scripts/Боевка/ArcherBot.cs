using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class ArcherBot : BotBase
{
    [Header("Archer Specific")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float arrowSpeed = 10f;
    public float retreatDistance = 5f;
    public float detectionRadius = 12f;
    public float attackRange = 8f;
    public LayerMask obstacleMask;

    private NavMeshAgent agent;
    private Transform target;
    private bool isReloading;

    private Vector3 wanderDir;
    private float nextWanderTime;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (healthSystem.IsDead())
        {
            agent.isStopped = true;
            return;
        }

        target = FindNearestTarget();

        if (target == null)
            Wander();
        else if (isReloading)
            Retreat();
        else if (HasLineOfSight(target) && Vector3.Distance(transform.position, target.position) <= attackRange)
            Attack();
        else
            Chase();
    }


    private Transform FindNearestTarget()
    {
        HealthSystem[] all = FindObjectsOfType<HealthSystem>();
        Transform best = null;
        float bestDist = detectionRadius;
        foreach (var h in all)
        {
            if (h == healthSystem || h.IsDead() || h.myType != healthSystem.targetType)
                continue;
            float d = Vector3.Distance(transform.position, h.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = h.transform;
            }
        }
        return best;
    }

    private void Wander()
    {
        agent.isStopped = false;
        if (Time.time >= nextWanderTime)
        {
            Vector2 rnd = Random.insideUnitCircle.normalized;
            wanderDir = new Vector3(rnd.x, rnd.y, 0f);
            nextWanderTime = Time.time + Random.Range(1f, 3f);
        }
        Vector3 dest = transform.position + wanderDir * (detectionRadius * 0.5f);
        if (NavMesh.SamplePosition(dest, out var hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    private void Chase()
    {
        if (target == null) return;
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    protected override void AttackLogic()
    {
        if (target == null) return;
        isReloading = true;
        agent.isStopped = true;

        var arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        if (arrow.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = (target.position - shootPoint.position).normalized * arrowSpeed;
        if (arrow.TryGetComponent<Arrow>(out var a))
            a.Init(this);

        Invoke(nameof(EndReload), attackCooldown);
    }

    private void EndReload() => isReloading = false;

    private void Retreat()
    {
        agent.isStopped = false;
        Vector3 dir = (transform.position - target.position).normalized;
        Vector3 dst = transform.position + dir * retreatDistance;
        if (NavMesh.SamplePosition(dst, out var hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    private bool HasLineOfSight(Transform t)
    {
        Vector2 start = shootPoint.position;
        Vector2 end = t.position;
        Vector2 dir = (end - start).normalized;
        float dist = Vector2.Distance(start, end);
        return !Physics2D.Raycast(start, dir, dist, obstacleMask);
    }
}