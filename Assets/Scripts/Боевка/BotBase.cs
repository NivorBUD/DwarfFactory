using UnityEngine;

public abstract class BotBase : MonoBehaviour, IDamageSource
{
    [Header("Combat")]
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    protected HealthSystem healthSystem;
    protected float lastAttackTime;

    protected virtual void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    public bool CanAttack() => Time.time >= lastAttackTime + attackCooldown;

    public void Attack()  // resets cooldown and invokes logic
    {
        lastAttackTime = Time.time;
        AttackLogic();
    }

    protected abstract void AttackLogic();

    // IDamageSource
    public int GetDamage() => attackDamage;
    public TargetType GetSourceType() => healthSystem.myType;
}
