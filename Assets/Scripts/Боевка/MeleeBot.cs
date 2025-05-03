using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class MeleeBot : BotBase
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;

    [Header("Melee Specific")]
    public float pushBackDistance = 0.5f;
    public float pushBackTime = 0.2f;

    private NavMeshAgent agent;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (healthSystem.IsDead())
        {
            agent.isStopped = true;
            return;
        }

        // Каждый кадр ищем ближайшую цель
        target = FindNearestTarget();

        if (target == null)
        {
            Wander();
        }
        else
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else if (CanAttack())
            {
                agent.isStopped = true;
                Attack();
            }
        }
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
        Vector2 rnd2 = Random.insideUnitCircle.normalized * detectionRadius;
        Vector3 dest = transform.position + new Vector3(rnd2.x, rnd2.y, 0f);
        if (NavMesh.SamplePosition(dest, out var hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    protected override void AttackLogic()
    {
        if (target == null) return;
        var hs = target.GetComponent<HealthSystem>();
        if (hs == null) return;

        hs.TakeDamage(GetDamage());
        if (hs.IsDead())
        {
            // сбрасываем цель
            target = null;
        }
        else
        {
            StartCoroutine(PushBack());
        }
    }

    private IEnumerator PushBack()
    {
        Vector3 start = target.position;
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 end = start + dir * pushBackDistance;
        float t = 0f;
        agent.isStopped = true;
        while (t < pushBackTime)
        {
            target.position = Vector3.Lerp(start, end, t / pushBackTime);
            t += Time.deltaTime;
            yield return null;
        }
        target.position = end;
        agent.isStopped = false;
    }
}