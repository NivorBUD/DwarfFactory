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
    // Публичные события для UI и звуков
    public event Action<CraftingRecipe> OnCraftStarted;
    public event Action<int> OnQueueCountChanged;
    public event Action<CraftingTask> OnCraftProgress;
    public event Action<CraftingTask> OnCraftCompleted;


    protected Queue<CraftingTask> craftingQueue = new Queue<CraftingTask>();
    protected bool isCrafting = false;

    /// <summary>
    /// �������� ������ � ������� (���� ������(�) ����, � ������ ������ ��� ����� �������).
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
    }

    protected abstract bool HasRequiredItems(CraftingRecipe recipe);
    protected abstract void RemoveIngredients(CraftingRecipe recipe);
    protected abstract void AddResult(CraftingRecipe recipe);


    /// <summary>
    /// �������� ��������, ������� ��������������� ��������� ������� �� �������.
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

            OnCraftStarted?.Invoke(recipe);
            RemoveIngredients(recipe);

            float timer = 0f;
            task.Progress = 0f;

            while (timer < recipe.craftingTime)
            {
                if (!isCrafting)
                    break;
                timer += Time.deltaTime;
                task.Progress = Mathf.Clamp01(timer / recipe.craftingTime);
                OnCraftProgress?.Invoke(task);
                yield return null;
            }

            AddResult(recipe);
            OnCraftCompleted?.Invoke(task);
            craftingQueue.Dequeue();
        }

        isCrafting = false;
    }


    /// <summary>
    /// �������� ������� � ���������� ���������.
    /// </summary>
    public void ClearQueue()
    {
        craftingQueue.Clear();
        if (isCrafting)
        {
            isCrafting = false;
            StopAllCoroutines();
            StopCoroutine(ProcessQueue());
        }
    }
}
