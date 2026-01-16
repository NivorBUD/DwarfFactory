using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingsGrid : MonoBehaviour
{
    public static BuildingsGrid Instance;
    public bool IsPlacingBuilding { get; private set; }

    [SerializeField] private Tilemap buildingsTilemap;

    private Camera cam;
    private Grid grid;
    private Building flyingBuilding;
    private SpriteRenderer buildingSpriteRenderer;
    private Dictionary<Vector3Int, Vector2Int> busyPositions = new(); // positition left bottom : size

    private void Awake()
    {
        cam = Camera.main;
        grid = GetComponent<Grid>();
        Instance = this;
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding);
        }

        flyingBuilding = Instantiate(buildingPrefab);
        
        // Находим дочерний объект Sprite
        Transform spriteTransform = flyingBuilding.transform.Find("Sprite");
        if (spriteTransform != null)
        {
            buildingSpriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        }
        
        // Отключаем все коллайдеры у здания во время размещения
        DisableColliders(flyingBuilding.gameObject);
        
        IsPlacingBuilding = true;
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            if (Mouse.current == null) return;

            Vector3 MousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int pos = grid.WorldToCell(MousePos);
            
            // Привязываем здание к сетке, но центрируем его относительно клетки под курсором
            Vector3 cellCenter = grid.CellToWorld(pos);
            cellCenter.x += 0.5f; // Центр клетки
            cellCenter.y += 0.5f; // Центр клетки
            flyingBuilding.transform.position = cellCenter;
            
            // Проверяем можно ли установить здание и меняем цвет
            bool canPlace = CheckToPlaceBuilding(pos);
            UpdateBuildingColor(canPlace);
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            { 
                if (canPlace)
                {
                    GameObject placedBuilding = Instantiate(flyingBuilding.gameObject);
                    
                    // Возвращаем исходный цвет установленному зданию
                    Transform placedSprite = placedBuilding.transform.Find("Sprite");
                    if (placedSprite != null)
                    {
                        SpriteRenderer sr = placedSprite.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.color = Color.white;
                        }
                    }
                    
                    // Включаем коллайдеры обратно у установленного здания
                    EnableColliders(placedBuilding);
                    
                    CreateTipGForBuilding(placedBuilding);
                    //buildingsTilemap.SetTile(grid.WorldToCell(MousePos), flyingBuilding.GetTile());
                    busyPositions.Add(pos, flyingBuilding.Size);
                    InventoryManager.Instance.RemoveUsedItemFromActiveSlot();
                }
            }
        }
    }

    public void StopPlacingBuilding()
    {
        Destroy(flyingBuilding.gameObject);
        buildingSpriteRenderer = null;
        IsPlacingBuilding = false;
    }

    private void UpdateBuildingColor(bool canPlace)
    {
        if (buildingSpriteRenderer == null) return;

        if (canPlace)
        {
            // Зеленый оттенок
            buildingSpriteRenderer.color = new Color(0.5f, 1f, 0.5f, 1f);
        }
        else
        {
            // Красный оттенок
            buildingSpriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f);
        }
    }

    private bool CheckToPlaceBuilding(Vector3Int gridPlace)
    {
        Rect buildingRect = new(gridPlace.x, gridPlace.y, flyingBuilding.Size.x, flyingBuilding.Size.y);
        
        // Проверка пересечения с другими зданиями
        foreach (Vector3Int pos in busyPositions.Keys)
        {
            Rect rect2 = new(pos.x, pos.y, busyPositions[pos].x, busyPositions[pos].y);
            if (buildingRect.Overlaps(rect2))
            {
                return false;
            }
        }

        // Проверка пересечения с игроком
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3Int playerGridPos = grid.WorldToCell(player.transform.position);
            // Считаем что игрок занимает 1 клетку
            Rect playerRect = new(playerGridPos.x, playerGridPos.y, 1, 1);
            
            if (buildingRect.Overlaps(playerRect))
            {
                return false;
            }
        }

        return true;
    }

    private void CreateTipGForBuilding(GameObject building)
    {
        // Проверяем, нет ли уже tipG
        Transform existingTipG = building.transform.Find("tipG");
        if (existingTipG != null) return;

        // Создаем новый GameObject для подсказки
        GameObject tipG = new GameObject("tipG");
        tipG.transform.SetParent(building.transform);
        tipG.transform.localPosition = new Vector3(0, 1f, 0); // Позиция над зданием
        tipG.transform.localScale = Vector3.one;

        // Добавляем SpriteRenderer
        SpriteRenderer spriteRenderer = tipG.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 100; // Чтобы был поверх всего

        // Загружаем спрайт из Resources (если есть)
        Sprite tipSprite = Resources.Load<Sprite>("tipG");
        if (tipSprite != null)
        {
            spriteRenderer.sprite = tipSprite;
        }
        else
        {
            // Если спрайта нет, создаем простой квадрат с текстом "G"
            // Можно также добавить TextMeshPro компонент
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
        }

        // По умолчанию скрываем подсказку
        tipG.SetActive(false);
    }

    private void DisableColliders(GameObject obj)
    {
        // Отключаем все коллайдеры на объекте и его дочерних объектах
        Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void EnableColliders(GameObject obj)
    {
        // Включаем все коллайдеры на объекте и его дочерних объектах
        Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
    }
}
