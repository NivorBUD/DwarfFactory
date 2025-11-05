using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingTask
{
    public CraftingRecipe Recipe { get; private set; }
    public float Progress { get; set; }

    public CraftingTask(CraftingRecipe recipe)
    {
        Recipe = recipe;
        Progress = 0f;
    }
}

public abstract class BaseCraftingSystem : MonoBehaviour
{
    protected Queue<CraftingTask> craftingQueue = new Queue<CraftingTask>();
    protected bool isCrafting = false;

    public virtual void QueueCraft(CraftingRecipe recipe)
    {
        craftingQueue.Enqueue(new CraftingTask(recipe));
        if (!isCrafting)
            StartCoroutine(ProcessQueue());
    }

    protected abstract bool HasRequiredItems(CraftingRecipe recipe);
    protected abstract void RemoveIngredients(CraftingRecipe recipe);
    protected abstract void AddResult(CraftingRecipe recipe);

    private IEnumerator ProcessQueue()
    {
        isCrafting = true;

        while (craftingQueue.Count > 0)
        {
            CraftingTask current = craftingQueue.Peek();
            CraftingRecipe recipe = current.Recipe;

            if (!HasRequiredItems(recipe))
            {
                craftingQueue.Dequeue();
                continue;
            }

            RemoveIngredients(recipe);

            float timer = 0f;
            while (timer < recipe.craftingTime)
            {
                timer += Time.deltaTime;
                current.Progress = Mathf.Clamp01(timer / recipe.craftingTime);
                OnCraftProgress(current);
                yield return null;
            }

            AddResult(recipe);
            OnCraftCompleted(current);
            craftingQueue.Dequeue();
        }

        isCrafting = false;
        yield return new WaitForSeconds(0.1f);
    }

    protected virtual void OnCraftProgress(CraftingTask task) { }
    protected virtual void OnCraftCompleted(CraftingTask task) { }
}
