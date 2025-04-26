using UnityEngine;

public abstract class BotBase : MonoBehaviour
{
    [Header("Параметры бота")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Поиск цели")]
    public float detectionRadius = 10f; // Радиус поиска врага

    protected Transform targetEnemy;
    protected Rigidbody2D rb;
    protected HealthSystem healthSystem;

    private float lastAttackTime;

    // Для хаотичного блуждания
    private Vector2 wanderDirection;
    private float wanderChangeTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
    }

    protected virtual void Update()
    {
        if (healthSystem == null || healthSystem.IsDead())
            return;

        FindNearestTarget();

        if (targetEnemy != null)
        {
            MoveLogic();
            TryAttack();
        }
        else
        {
            Wander();
        }
    }

    protected abstract void MoveLogic(); // Потомки реализуют
    protected abstract void Attack();    // Потомки реализуют

    protected virtual void TryAttack()
    {
        if (targetEnemy == null)
            return;

        float distance = Vector2.Distance(transform.position, targetEnemy.position);
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected virtual void FindNearestTarget()
    {
        HealthSystem[] allTargets = GameObject.FindObjectsOfType<HealthSystem>();
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (var target in allTargets)
        {
            if (target == null || target == healthSystem || target.IsDead())
                continue;

            if (target.myType == healthSystem.myType) // Свои - пропускаем
                continue;

            if (targetTypeMatch(target))
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (dist < detectionRadius && dist < closestDistance) // <<< Условие по радиусу
                {
                    closestDistance = dist;
                    closestTarget = target.transform;
                }
            }
        }

        targetEnemy = closestTarget;
    }

    protected virtual bool targetTypeMatch(HealthSystem target)
    {
        return healthSystem.targetType == target.myType;
    }


    protected virtual void Wander()
    {
        if (Time.time > wanderChangeTime)
        {
            wanderDirection = Random.insideUnitCircle.normalized;
            wanderChangeTime = Time.time + Random.Range(1f, 3f);
        }

        rb.MovePosition(rb.position + wanderDirection * moveSpeed * 0.3f * Time.deltaTime);
    }
}
