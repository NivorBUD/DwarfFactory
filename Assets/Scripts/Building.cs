using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    public Vector2Int Size = Vector2Int.one;

    [SerializeField]
    private Tile tile;

    internal TileBase GetTile()
    {
        return tile;
    }

    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < Size.x; ++x)
        {
            for (int y = 0; y < Size.y; ++y)
            {
                Gizmos.color = new Color(233, 0, 0, 0.3f);
                Gizmos.DrawCube(transform.position + new Vector3(x, y, 0), new Vector3(1, 1, 0.1f));
            }
        }
    }
}
