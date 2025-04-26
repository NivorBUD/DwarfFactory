using UnityEngine;

public class MeleeBot : BotBase
{
    protected override void MoveLogic()
    {
        if (targetEnemy == null)
            return;

        float distance = Vector2.Distance(transform.position, targetEnemy.position);

        if (distance > attackRange * 0.8f)
        {
            Vector2 direction = ((Vector2)targetEnemy.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    protected override void Attack()
    {
        if (targetEnemy == null)
            return;

        HealthSystem enemyHealth = targetEnemy.GetComponent<HealthSystem>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            // Тут можно добавить анимацию удара или эффект удара
        }
    }
}
