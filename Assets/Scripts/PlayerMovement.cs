using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Интерфейс для интерактивных объектов
public interface IInteractable
{
    void Interact(GameObject interactor);
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Animation & Sound")]
    [SerializeField] private PlayerAnimationController animController;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private InputSystem_Actions controls;
    private bool isInteracting = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();

        // Настройка физики
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Автоподхват аниматора
        if (animController == null)
            animController = GetComponent<PlayerAnimationController>();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Interact.performed += OnInteract;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Interact.performed -= OnInteract;
        controls.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (isInteracting) return;
        StartCoroutine(InteractCoroutine());
    }

    private IEnumerator InteractCoroutine()
    {
        isInteracting = true;
        animController.PlayInteract(true);

        // Поиск всех IInteractable рядом
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
        foreach (var hit in hits)
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact(gameObject);
                break;
            }

        // Ждём окончания анимации (1 сек)
        yield return new WaitForSeconds(1f);

        animController.PlayInteract(false);
        isInteracting = false;
    }

    private void FixedUpdate()
    {
        if (isInteracting)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            MoveCharacter();
        }
    }

    private void MoveCharacter()
    {
        Vector2 targetVel = moveInput.normalized * moveSpeed;
        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVel,
            (targetVel.magnitude > currentVelocity.magnitude ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        rb.linearVelocity = currentVelocity;

        // Передаём скорость в анимацию
        animController.UpdateSpeed(currentVelocity.magnitude);

        // Флип по X
        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
