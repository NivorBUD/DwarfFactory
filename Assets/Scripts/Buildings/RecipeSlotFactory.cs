using UnityEngine;
using UnityEngine.UI;
using System;

public class RecipeSlotFactory
{
    private readonly GameObject _slotPrefab;
    private readonly Transform _container;

    public RecipeSlotFactory(GameObject slotPrefab, Transform container)
    {
        _slotPrefab = slotPrefab;
        _container = container;
    }

    //public void CreateSlots(RecipeScriptableObject[] recipes, Action<RecipeScriptableObject> onSelect)
    //{
    //    foreach (var recipe in recipes)
    //    {
    //        var slotObj = UnityEngine.Object.Instantiate(_slotPrefab, _container);
    //        var slot = slotObj.GetComponent<RecipeSelectionSlot>();

    //        if (slot == null)
    //        {
    //            Debug.LogError("RecipeSlot prefab missing RecipeSelectionSlot component!");
    //            continue;
    //        }

    //        slot.Setup(recipe);

    //        var button = slotObj.GetComponent<Button>();
    //        if (button != null)
    //            button.onClick.AddListener(() => onSelect(recipe));
    //    }
    //}
}
