using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RecipeSelectionSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CraftingRecipe recipe;
    [SerializeField] private Image itemImage;

    public System.Action<CraftingRecipe> OnClick;

    public void SetRecipe(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;

        if (recipe == null)
        {
            itemImage.color = Color.clear;
            return;
        }

        GetComponent<Image>().sprite = recipe.resultItem.icon;
        GetComponent<Image>().color = Color.white;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (recipe == null)
            return;

        OnClick?.Invoke(recipe);
    }
}