using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Компонент для отображения подсказки с информацией о предмете
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private RectTransform tooltipRect;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(10, -10);

    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        canvas = GetComponentInParent<Canvas>();
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void ShowTooltip(ItemScriptableObject item, Vector2 position)
    {
        if (item == null || tooltipPanel == null) return;

        // Устанавливаем данные предмета
        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.itemDescription;

        if (itemIconImage != null && item.icon != null)
        {
            itemIconImage.sprite = item.icon;
            itemIconImage.gameObject.SetActive(true);
        }
        else if (itemIconImage != null)
        {
            itemIconImage.gameObject.SetActive(false);
        }

        // Показываем панель
        tooltipPanel.SetActive(true);

        // Позиционируем tooltip
        UpdatePosition(position);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        if (tooltipRect == null || canvas == null) return;

        // Конвертируем позицию мыши в позицию на canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            position,
            canvas.worldCamera,
            out localPoint
        );

        // Применяем смещение
        localPoint += offset;

        // Устанавливаем позицию
        tooltipRect.localPosition = localPoint;

        // Проверяем, не выходит ли tooltip за границы экрана
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        if (tooltipRect == null || canvas == null) return;

        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        // Проверяем правую границу
        if (corners[2].x > canvasCorners[2].x)
        {
            float diff = corners[2].x - canvasCorners[2].x;
            tooltipRect.position = new Vector3(tooltipRect.position.x - diff, tooltipRect.position.y, tooltipRect.position.z);
        }

        // Проверяем нижнюю границу
        if (corners[0].y < canvasCorners[0].y)
        {
            float diff = canvasCorners[0].y - corners[0].y;
            tooltipRect.position = new Vector3(tooltipRect.position.x, tooltipRect.position.y + diff, tooltipRect.position.z);
        }
    }
}
