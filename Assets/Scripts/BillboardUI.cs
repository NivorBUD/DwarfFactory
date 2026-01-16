using UnityEngine;

/// <summary>
/// Компонент для UI элементов, которые не должны поворачиваться вместе с родителем
/// Компенсирует изменение scale.x родительского объекта
/// </summary>
public class BillboardUI : MonoBehaviour
{
    private Transform parentTransform;
    private Vector3 originalScale;

    private void Start()
    {
        parentTransform = transform.parent;
        originalScale = transform.localScale;
    }

    private void LateUpdate()
    {
        if (parentTransform == null) return;

        // Компенсируем поворот родителя по оси X
        Vector3 newScale = originalScale;
        
        // Если родитель перевернут (отрицательный scale.x), переворачиваем обратно
        if (parentTransform.localScale.x < 0)
        {
            newScale.x = -Mathf.Abs(originalScale.x);
        }
        else
        {
            newScale.x = Mathf.Abs(originalScale.x);
        }

        transform.localScale = newScale;
    }
}
