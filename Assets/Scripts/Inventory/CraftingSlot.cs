using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private CraftingRecipe Recipe;

    private Sprite iconSprite;     // картинка иконки результата
    private Sprite emptySprite;
    [SerializeField]private Image itemImage;       // фон слота (также ловит клики)
    private TextMeshProUGUI amountText; // сколько штук можно крафтить

    private bool isCrafting = false;

    private void Start()
    {
        amountText = transform.GetComponentInChildren<TextMeshProUGUI>();
        emptySprite = itemImage.sprite;
        SetRecipe(Recipe);
    }

    /// <summary>
    /// Задаёт рецепт и обновляет UI
    /// </summary>
    public void SetRecipe(CraftingRecipe recipe)
    {
        Recipe = recipe;

        if (Recipe == null)
        {
            ClearSlot();
            return;
        }

        iconSprite = Recipe.resultItem.icon;
        itemImage.sprite = iconSprite;
        itemImage.color = Color.white;

        RefreshAmountText();
    }

    private void ClearSlot()
    {
        Recipe = null;
        iconSprite = null;
        amountText.text = "";
        itemImage.color = Color.clear;
    }

    public void RefreshAmountText()
    {
        if (Recipe == null)
        {
            amountText.text = "";
            return;
        }

        int maxCrafts = CraftingSystem.Instance.CalculateMaxCrafts(Recipe);
        if (maxCrafts > 0)
        {
            amountText.text = maxCrafts.ToString();
        }
        else
        {
            amountText.text = maxCrafts == -1 ? "" : maxCrafts.ToString();
        }
    }

    /// <summary>
    /// Это метод из IPointerClickHandler, ловит нажатие по itemImage или любому Raycast-Target UI-элементу на этом объекте
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Recipe == null || isCrafting)
            return;

        int maxCrafts = CraftingSystem.Instance.CalculateMaxCrafts(Recipe);
        if (maxCrafts == 0)
            return;

        Craft();
    }

    private void Craft()
    {
        isCrafting = true;
        // Можно тут включить прогресс-бар/анимацию

        bool ok = CraftingSystem.Instance.Craft(Recipe);
        if (!ok)
            Debug.LogWarning($"Не удалось скрафтить {Recipe.resultItem.itemName}");

        isCrafting = false;
        RefreshAmountText();
    }
}
