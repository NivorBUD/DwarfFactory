using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Компонент для слотов инвентаря, который показывает tooltip при наведении
/// Добавьте этот компонент на каждый слот инвентаря
/// </summary>
public class ItemTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private InventorySlot slot;
    private bool isHovering = false;

    private void Awake()
    {
        slot = GetComponent<InventorySlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot != null && slot.Item != null && ItemTooltip.Instance != null)
        {
            isHovering = true;
            ItemTooltip.Instance.ShowTooltip(slot.Item, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.HideTooltip();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isHovering && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.UpdatePosition(eventData.position);
        }
    }

    private void OnDisable()
    {
        // Скрываем tooltip если слот был отключен
        if (isHovering && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.HideTooltip();
            isHovering = false;
        }
    }
}
