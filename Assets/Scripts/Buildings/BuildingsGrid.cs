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
        IsPlacingBuilding = true;
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            if (Mouse.current == null) return;

            Vector3 MousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int pos = grid.WorldToCell(MousePos);
            flyingBuilding.transform.position = new Vector3Int(pos.x + 1, pos.y + 1, 0);
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            { 
                bool canPlace = CheckToPlaceBuilding(pos);
                if (canPlace)
                {
                    GameObject placedBuilding = Instantiate(flyingBuilding.gameObject);
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
        IsPlacingBuilding = false;
    }

    private bool CheckToPlaceBuilding(Vector3Int gridPlace)
    {
        Rect rect1 = new(gridPlace.x, gridPlace.y, flyingBuilding.Size.x, flyingBuilding.Size.y);
        foreach (Vector3Int pos in busyPositions.Keys)
        {
            Rect rect2 = new(pos.x, pos.y, busyPositions[pos].x, busyPositions[pos].y);
            if (rect1.Overlaps(rect2))
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
}
