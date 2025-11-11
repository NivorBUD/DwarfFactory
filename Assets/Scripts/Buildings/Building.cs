using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

abstract public class Building : MonoBehaviour
{
    public Vector2Int Size = Vector2Int.one;

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

    protected virtual void OnEnable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnBuildingInteract += HandleInteraction;
        }
    }

    protected virtual void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnBuildingInteract -= HandleInteraction;
        }
    }

    private void HandleInteraction()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            interaction();
        }
    }

    abstract public void interaction();
}
