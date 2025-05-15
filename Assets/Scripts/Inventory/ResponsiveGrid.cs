using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveInventory : MonoBehaviour
{
    public RectTransform container;
    public GridLayoutGroup gridLayout;
    public int totalSlots = 56;
    public int baseColumns = 7;
    public float spacing = 5f;
    public float padding = 10f;

    private void Start()
    {
        UpdateLayout();
    }

    private void Update()
    {
        UpdateLayout();
    }

    void UpdateLayout()
    {
        float containerWidth = container.rect.width;
        float containerHeight = container.rect.height;

        int columns = baseColumns;
        int rows = Mathf.CeilToInt((float)totalSlots / columns);

        float totalSpacingWidth = (columns - 1) * spacing + padding * 2;
        float totalSpacingHeight = (rows - 1) * spacing + padding * 2;

        float cellWidth = (containerWidth - totalSpacingWidth) / columns;
        float cellHeight = (containerHeight - totalSpacingHeight) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.spacing = new Vector2(spacing, spacing);
        gridLayout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
    }
}