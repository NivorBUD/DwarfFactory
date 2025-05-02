using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<RecipeIngredient> ingredients;
    public ItemScriptableObject resultItem;
    public int resultAmount = 1;
    public float craftingTime = 1f;
}

[System.Serializable]
public class RecipeIngredient
{
    public ItemScriptableObject item;
    public int amount;
}
