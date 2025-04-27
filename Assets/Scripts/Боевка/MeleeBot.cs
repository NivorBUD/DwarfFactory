using UnityEngine;

public class MeleeBot : BotBase
{
    [Header("Melee Specific")]
    public int attackDamage = 10;
    public float pushBackForce = 0.5f;

    protected override void AttackLogic()
    {
        if (target == null) return;
        var h = target.GetComponent<HealthSystem>();
        if (h != null)
        {
            h.TakeDamage(attackDamage);

            if (h.IsDead())
            {
                OnTargetKilled();
            }
            else
            {
                // Пушбек только если жив
                Vector2 away = ((Vector2)transform.position - (Vector2)target.position).normalized;
                transform.position += (Vector3)(away * pushBackForce);
            }
        }
    }

}
