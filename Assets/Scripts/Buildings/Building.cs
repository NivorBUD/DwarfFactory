using System;
using System.Collections;
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
        // Подписываемся сразу если InputHandler уже есть
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInteract += TryInteract;
        }
        else
        {
            // Если InputHandler еще не создан, подпишемся позже
            StartCoroutine(WaitForInputHandler());
        }
    }

    private IEnumerator WaitForInputHandler()
    {
        // Ждем пока InputHandler инициализируется
        while (InputHandler.Instance == null)
        {
            yield return null;
        }
        InputHandler.Instance.OnInteract += TryInteract;
    }

    protected virtual void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInteract -= TryInteract;
        }
    }

    private void TryInteract()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(player.transform.position, transform.position);
        
        if (distance <= 2f)
        {
            // Проверяем, нет ли более близкого здания
            Building[] allBuildings = FindObjectsOfType<Building>();
            Building closest = null;
            float closestDistance = float.MaxValue;

            foreach (Building building in allBuildings)
            {
                float dist = Vector2.Distance(player.transform.position, building.transform.position);
                if (dist <= 2f && dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = building;
                }
            }

            // Взаимодействуем только если это здание - самое близкое
            if (closest == this)
            {
                interaction();
            }
        }
    }

    abstract public void interaction();
}
