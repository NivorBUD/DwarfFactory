using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Контроллер взаимодействия с объектами (сундуки, боты) по кнопке G
/// Находит ближайший объект в радиусе и открывает его инвентарь
/// </summary>
public class InteractionController : MonoBehaviour
{
    public static InteractionController Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private Transform player;

    private GameObject currentNearestObject;
    private GameObject lastHighlightedObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Подписываемся на событие нажатия G
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInteract += HandleInteraction;
        }
    }

    private void Update()
    {
        // Постоянно обновляем ближайший объект и показываем tipG только на нем
        UpdateNearestObject();
    }

    private void UpdateNearestObject()
    {
        currentNearestObject = FindNearestInteractable();

        // Если ближайший объект изменился, обновляем подсказки
        if (currentNearestObject != lastHighlightedObject)
        {
            // Скрываем подсказку у предыдущего объекта
            if (lastHighlightedObject != null)
            {
                HideTipG(lastHighlightedObject);
            }

            // Показываем подсказку у нового объекта
            if (currentNearestObject != null)
            {
                ShowTipG(currentNearestObject);
            }

            lastHighlightedObject = currentNearestObject;
        }
    }

    private void ShowTipG(GameObject obj)
    {
        Transform tipG = obj.transform.Find("tipG");
        if (tipG != null)
        {
            tipG.gameObject.SetActive(true);
        }
    }

    private void HideTipG(GameObject obj)
    {
        Transform tipG = obj.transform.Find("tipG");
        if (tipG != null)
        {
            tipG.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInteract -= HandleInteraction;
        }

        // Скрываем подсказку при уничтожении
        if (lastHighlightedObject != null)
        {
            HideTipG(lastHighlightedObject);
        }
    }

    private void HandleInteraction()
    {
        // Если уже открыт сундук или инвентарь бота - закрываем
        if (InventoryManager.Instance != null && InventoryManager.Instance.ui.IsInventoryOpened)
        {
            if (InventoryManager.Instance.ui.IsChestOpened || InventoryManager.Instance.ui.IsDwarfOpened)
            {
                InventoryManager.Instance.CloseInventoryFromButton();
                return;
            }
        }

        // Используем текущий ближайший объект
        if (currentNearestObject != null)
        {
            // Проверяем Dwarf (бот)
            Dwarf dwarf = currentNearestObject.GetComponent<Dwarf>();
            if (dwarf != null)
            {
                dwarf.Interaction();
                return;
            }

            // Проверяем Building (сундук и другие здания)
            Building building = currentNearestObject.GetComponent<Building>();
            if (building != null)
            {
                building.interaction();
                return;
            }
        }
    }

    private GameObject FindNearestInteractable()
    {
        if (player == null) return null;

        List<GameObject> interactables = new List<GameObject>();

        // Находим все интерактивные объекты в радиусе
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.position, interactionRadius);
        
        foreach (var collider in colliders)
        {
            // Проверяем наличие компонентов Building (сундуки, крафт-станции) или Dwarf (боты)
            if (collider.GetComponent<Building>() != null || collider.GetComponent<Dwarf>() != null)
            {
                interactables.Add(collider.gameObject);
            }
        }

        if (interactables.Count == 0) return null;

        // Находим ближайший объект
        GameObject nearest = interactables
            .OrderBy(obj => Vector2.Distance(player.position, obj.transform.position))
            .FirstOrDefault();

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, interactionRadius);
    }
}
