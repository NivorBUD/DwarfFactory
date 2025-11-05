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

    private bool isCrafting = false;

    private void Start()
    {
        if (recipe != null)
            SetRecipe(recipe);
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
        RefreshAmountText();
    }

    public void RefreshAmountText()
    {
        int maxCrafts = InventoryManager.Instance != null
            ? InventoryManager.Instance.CalculateMaxCrafts(recipe)
            : 0;

        amountText.text = maxCrafts > 0 ? maxCrafts.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (recipe == null || isCrafting)
            return;

        PlayerCraftingSystem.Instance.QueueCraft(recipe);
        StartCoroutine(ShowProgress(recipe.craftingTime));
    }

    private IEnumerator ShowProgress(float duration)
    {
        isCrafting = true;
        float t = 0f;
        progressOverlay.fillAmount = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            progressOverlay.fillAmount = t / duration;
            yield return null;
        }

        progressOverlay.fillAmount = 1f;
        yield return new WaitForSeconds(0.1f);
        progressOverlay.fillAmount = 0f;
        isCrafting = false;
        RefreshAmountText();
    }
}
