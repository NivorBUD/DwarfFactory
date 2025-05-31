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

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private InputSystem_Actions controls;
    private Animator animator;

    private bool isInteracting = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
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

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInteracting) return;
        StartCoroutine(InteractCoroutine());
    }

    private IEnumerator InteractCoroutine()
    {
        isInteracting = true;
        animator.SetBool("IsInteracting", true);

        // Попытка найти объект для взаимодействия
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                break;
            }
        }

        // Даже если ничего не найдено — просто играем анимацию
        yield return new WaitForSeconds(1f); // длительность взаимодействия

        animator.SetBool("IsInteracting", false);
        isInteracting = false;
    }


    private void FixedUpdate()
    {
        if (!isInteracting)
            MoveCharacter();
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void MoveCharacter()
    {
        Vector2 targetVelocity = moveInput.normalized * moveSpeed;

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > currentVelocity.magnitude ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        rb.linearVelocity = currentVelocity;

        // Анимация
        //animator.SetFloat("Speed", currentVelocity.magnitude);

        // Поворот персонажа по X
        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1); // вправо (задом)
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(1, 1, 1); // влево (лицом)
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
