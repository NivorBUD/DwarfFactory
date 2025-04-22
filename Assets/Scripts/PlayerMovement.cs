using UnityEngine;
using UnityEngine.InputSystem;

// Интерфейс для интерактивных объектов
public interface IInteractable
{
    void Interact(GameObject interactor);
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Максимальная скорость
    [SerializeField] private float acceleration = 50f; // Ускорение
    [SerializeField] private float deceleration = 50f; // Замедление (инерция)

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 1f; // Радиус взаимодействия
    [SerializeField] private LayerMask interactableLayer; // Слой для интерактивных объектов

    private Vector2 moveInput; // Ввод с Input System для движения
    private Vector2 currentVelocity; // Текущая скорость
    private Rigidbody2D rb;
    private InputSystem_Actions controls; // Ваш класс Input Actions

    private void Awake()
    {
        // Инициализация Input System
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();

        // Настройка Rigidbody2D
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // Запрет вращения
    }

    private void OnEnable()
    {
        // Подписка на ввод движения
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        // Подписка на ввод взаимодействия
        controls.Player.Interact.performed += OnInteract;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        // Отписка от ввода
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Interact.performed -= OnInteract;
        controls.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Получение вектора движения (WASD/геймпад)
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // Проверка ближайших интерактивных объектов
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                break; // Взаимодействуем только с первым найденным объектом
            }
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        // Целевая скорость на основе ввода
        Vector2 targetVelocity = moveInput.normalized * moveSpeed;

        // Плавное ускорение/замедление
        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > currentVelocity.magnitude ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        // Применение скорости к Rigidbody2D
        rb.linearVelocity = currentVelocity;
    }

    // Визуализация радиуса взаимодействия в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}