using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

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
    // ¬нешние событи€ дл€ UI
    public event Action<CraftingRecipe> OnCraftStarted;
    public event Action<int> OnQueueCountChanged;
    public event Action<CraftingTask> OnCraftProgress;
    public event Action<CraftingTask> OnCraftCompleted;


    protected Queue<CraftingTask> craftingQueue = new Queue<CraftingTask>();
    protected bool isCrafting = false;

    /// <summary>
    /// ƒобавить рецепт в очередь (если ресурс(ы) есть, в момент старта они будут списаны).
    /// </summary>
    public virtual void QueueCraft(CraftingRecipe recipe)
    {
        if (recipe == null) return;
        craftingQueue.Enqueue(new CraftingTask(recipe));
        OnQueueCountChanged?.Invoke(craftingQueue.Count);

        if (!isCrafting)
        {
            StartCoroutine(ProcessQueue());
        }

        craftingQueue.Enqueue(new CraftingTask(recipe));
        if (!isCrafting)
            StartCoroutine(ProcessQueue());
    }

    protected abstract bool HasRequiredItems(CraftingRecipe recipe);
    protected abstract void RemoveIngredients(CraftingRecipe recipe);
    protected abstract void AddResult(CraftingRecipe recipe);


    /// <summary>
    /// ќсновна€ корутина, котора€ последовательно выполн€ет задани€ из очереди.
    /// </summary>
    protected IEnumerator ProcessQueue()
    {
        isCrafting = true;

        while (craftingQueue.Count > 0)
        {
            CraftingTask task = craftingQueue.Peek();
            CraftingRecipe recipe = task.Recipe;

            if (!HasRequiredItems(recipe))
            {
                craftingQueue.Dequeue();
                continue;
            }

            RemoveIngredients(recipe);

            float timer = 0f;
            task.Progress = 0f;
            while (timer < recipe.craftingTime)
            {
                timer += Time.deltaTime;
                task.Progress = Mathf.Clamp01(timer / recipe.craftingTime);
                OnCraftProgress(task);
                yield return null;
            }

            AddResult(recipe);
            OnCraftCompleted(task);
            craftingQueue.Dequeue();
        }

        isCrafting = false;
        yield return new WaitForSeconds(0.1f);
    }


}
