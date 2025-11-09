using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CraftingRecipe recipe;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image progressOverlay; // полупрозрачная заливка прогресса
    [SerializeField] private TextMeshProUGUI amountText;

    private void Start()
    {
        SetRecipe(recipe);

        progressOverlay.color = Color.clear;
    }

    public void SetRecipe(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;

        if (recipe == null)
        {
            itemImage.color = Color.clear;
            amountText.text = "";
            return;
        }

        itemImage.sprite = recipe.resultItem.icon;
        itemImage.color = Color.white;
        RefreshAmountText(null);
    }

    public void RefreshAmountText(CraftingTask task)
    {
        if (recipe == null) return;

        int maxCrafts = InventoryManager.Instance != null
            ? InventoryManager.Instance.CalculateMaxCrafts(recipe)
            : 0;

        amountText.text = maxCrafts > 0 ? maxCrafts.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (recipe == null)
            return;

        PlayerCraftingSystem.Instance.QueueCraft(recipe);
    }

    public void UpdateProgress(CraftingTask task)
    {
        progressOverlay.color = new Color(1, 0.3f, 0.3f, 1 - task.Progress);
    }
}
