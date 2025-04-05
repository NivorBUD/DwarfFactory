using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;

public class BuildingsGrid : MonoBehaviour
{
    [SerializeField]
    private Tilemap buildingsTilemap;

    private Camera cam;
    private Grid grid;
    private Building flyingBuilding;
    private bool[,] map; // надо доделать, это дл€ корректной проверки зан€тости, когда ставим предмет, то тут занимаем облость, через зан€тие тайлов не удобно провер€ть

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
                bool canPlace = CheckTileToPlaceBuilding(pos);
                Debug.Log(canPlace);
                if (canPlace)
                {
                    buildingsTilemap.SetTile(grid.WorldToCell(MousePos), flyingBuilding.GetTile());
                    StopPlacingBuilding();
                }
            }
        }
    }

    private void StopPlacingBuilding()
    {
        Destroy(flyingBuilding.gameObject);
    }

    private bool CheckTileToPlaceBuilding(Vector3Int gridPlace)
    {
        for (int i = 0; i < flyingBuilding.Size.x; i++)
        {
            for (int j = 0; j < flyingBuilding.Size.y; j++)
            {
                if (buildingsTilemap.HasTile(gridPlace + new Vector3Int(i, j, 0)))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
