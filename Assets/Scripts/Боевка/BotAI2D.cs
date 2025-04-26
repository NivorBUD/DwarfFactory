using UnityEngine;

public class BotAI2D : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public LayerMask enemyLayer;

    private Transform targetEnemy;

    void Update()
    {
        FindEnemy();

        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
        }
    }

    void FindEnemy()
    {
        if (targetEnemy == null)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

            if (enemies.Length > 0)
            {
                targetEnemy = enemies[0].transform; // Просто первый найденный враг
            }
        }
        else
        {
            // Проверка: если враг умер, забываем его
            if (targetEnemy == null)
                targetEnemy = null;
        }
    }

    void MoveTowardsEnemy()
    {
        if (Vector2.Distance(transform.position, targetEnemy.position) > 1f) // Подходить на дистанцию удара
        {
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
