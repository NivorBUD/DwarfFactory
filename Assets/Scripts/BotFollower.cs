using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BotFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 randomOffset;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        // Случайное смещение для каждого бота
        randomOffset = Random.insideUnitCircle * 0.5f;
    }

    // --- Новый метод для обработки ввода ---
    private void Update()
    {
        // Проверяем, нажата ли клавиша "K"
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }
    // ---------------------------------------

    private void FixedUpdate()
    {
        if (player == null) return;
        // ... (остальной код FixedUpdate без изменений)
        
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 velocity = Vector2.zero;
        bool isMoving = false;

        if (distance > followDistance)
        {
            // Целевая позиция с учетом смещения
            Vector2 targetPos = (Vector2)player.position + randomOffset;
            
            // Двигаемся к игроку
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            velocity = direction * moveSpeed;
            isMoving = true;

            // Поворот к игроку (сохраняем Y и Z scale)
            if (direction.x > 0.01f)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < -0.01f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (distance < stopDistance)
        {
            // Слишком близко - отходим
            Vector2 direction = (transform.position - player.position).normalized;
            velocity = direction * moveSpeed * 0.5f;
            isMoving = true;
        }

        rb.linearVelocity = velocity;

        // Анимация
        if (animator != null)
        {
            animator.SetFloat("Speed", velocity.magnitude);
            animator.SetBool("IsWalking", isMoving);
        }
    }

    // --- Метод смерти, который будет использоваться в игре ---
    public void Die()
    {
        // 1. Останавливаем движение бота
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        // Отключаем этот компонент, чтобы остановить логику движения и вызов Die
        enabled = false; 

        // 2. Запускаем анимацию смерти
        if (animator != null)
        {
            // Убедитесь, что триггер "Die" настроен в Animator Controller!
            animator.SetTrigger("DeathTrigger"); 
        }

        // 3. Удаляем объект через время проигрывания анимации
        Destroy(gameObject, 3f); 
    }
    // --------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}