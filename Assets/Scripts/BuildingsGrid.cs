using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingsGrid : MonoBehaviour
{
    [SerializeField]
    private Tilemap buildingsTilemap;

    private Camera cam;
    private Grid grid;
    private Building flyingBuilding;
    private Dictionary<Vector3Int, Vector2Int> busyPositions = new(); // positition left bottom : size

    private void Awake()
    {
        cam = Camera.main;
        grid = GetComponent<Grid>();
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding);
        }

        flyingBuilding = Instantiate(buildingPrefab);
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            Vector3 MousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int pos = grid.WorldToCell(MousePos);
            flyingBuilding.transform.position = new Vector3Int(pos.x + 1, pos.y + 1, 0);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            { 
                bool canPlace = CheckToPlaceBuilding(pos);
                Debug.Log(canPlace);
                if (canPlace)
                {
                    buildingsTilemap.SetTile(grid.WorldToCell(MousePos), flyingBuilding.GetTile());
                    busyPositions.Add(pos, flyingBuilding.Size);
                    StopPlacingBuilding();
                }
            }
        }
    }

    private void StopPlacingBuilding()
    {
        Destroy(flyingBuilding.gameObject);
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
}
