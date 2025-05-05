using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveInventory : MonoBehaviour
{
    public RectTransform container;        // Контейнер для слотов
    public GridLayoutGroup gridLayout;     // GridLayoutGroup компонента
    public int totalSlots = 56;            // Всего слотов (7x8)
    public int baseColumns = 7;            // Изначальное количество столбцов
    public float spacing = 5f;             // Отступы между слотами
    public float padding = 10f;            // Внутренние отступы от краёв

    private int lastColumns = -1;

    private void Start()
    {
        UpdateLayout();
    }

    private void Update()
    {
        // Можно использовать OnRectTransformDimensionsChange() вместо Update() при оптимизации
        UpdateLayout();
    }

    void UpdateLayout()
    {
        float containerWidth = container.rect.width;

        // Вычисляем минимально возможную ширину слота
        float availableWidth = containerWidth - gridLayout.padding.left - gridLayout.padding.right;
        float maxCellWidth = (availableWidth - (baseColumns - 1) * gridLayout.spacing.x) / baseColumns;

        // Пробуем уменьшать количество столбцов, если ширины не хватает
        int columns = baseColumns;
        while (columns > 1)
        {
            float cellWidth = (availableWidth - (columns - 1) * spacing) / columns;
            if (cellWidth >= maxCellWidth * 0.85f) // не даём слоту стать слишком маленьким
                break;
            columns--;
        }

        if (columns != lastColumns)
        {
            lastColumns = columns;

            int rows = Mathf.CeilToInt((float)totalSlots / columns);
            float cellSize = Mathf.Min(
                (availableWidth - (columns - 1) * spacing) / columns,
                (container.rect.height - (rows - 1) * spacing) / rows
            );

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(spacing, spacing);
        }
    }
}