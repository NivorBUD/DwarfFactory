using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingBuildingUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider craftingProgress;
    [SerializeField] private Image craftingIconFill;
    [SerializeField] private TextMeshProUGUI queueCountText;
    [SerializeField] private Button addToQueueButton;
    [SerializeField] private Button clearQueueButton;

    private CraftingBuilding building;               // ссылка на CraftingBuilding
    private BuildingCraftingSystem craftingSystem;   // ссылка на систему крафта
    private CraftingRecipe currentRecipe;

    private void Awake()
    {
        // находим CraftingBuilding и систему на том же объекте
        building = GetComponentInParent<CraftingBuilding>();
        craftingSystem = GetComponentInParent<BuildingCraftingSystem>();

        if (craftingSystem == null || building == null)
        {
            Debug.LogError("[CraftingBuildingUI] Не найдены ссылки на систему или здание!");
            return;
        }

        // Подписка на события системы
        craftingSystem.OnCraftProgress += HandleCraftProgress;
        craftingSystem.OnCraftStarted += HandleCraftStarted;
        craftingSystem.OnCraftCompleted += HandleCraftCompleted;
        craftingSystem.OnQueueCountChanged += HandleQueueCountChanged;

        // Обработчики кнопок
        if (addToQueueButton != null)
            addToQueueButton.onClick.AddListener(OnAddToQueue);

        if (clearQueueButton != null)
            clearQueueButton.onClick.AddListener(OnClearQueue);
    }

    private void OnDestroy()
    {
        if (craftingSystem != null)
        {
            craftingSystem.OnCraftProgress -= HandleCraftProgress;
            craftingSystem.OnCraftStarted -= HandleCraftStarted;
            craftingSystem.OnCraftCompleted -= HandleCraftCompleted;
            craftingSystem.OnQueueCountChanged -= HandleQueueCountChanged;
        }
    }

    // ==== Обработчики событий BuildingCraftingSystem ====

    private void HandleCraftProgress(CraftingTask task)
    {
        if (craftingProgress != null)
            craftingProgress.value = task.Progress;

        if (craftingIconFill != null && task.Recipe != null)
        {
            craftingIconFill.fillAmount = task.Progress;
            craftingIconFill.sprite = task.Recipe.resultItem.icon;
            craftingIconFill.color = Color.white;
        }
    }

    private void HandleCraftStarted(CraftingRecipe recipe)
    {
        currentRecipe = recipe;
        if (craftingProgress != null)
        {
            craftingProgress.gameObject.SetActive(true);
            craftingProgress.value = 0f;
        }

        if (craftingIconFill != null)
        {
            craftingIconFill.sprite = recipe.resultItem.icon;
            craftingIconFill.color = Color.white;
            craftingIconFill.fillAmount = 0f;
        }
    }

    private void HandleCraftCompleted(CraftingTask task)
    {
        if (craftingProgress != null)
            craftingProgress.value = 0f;

        if (craftingIconFill != null)
            craftingIconFill.fillAmount = 0f;
    }

    private void HandleQueueCountChanged(int count)
    {
        if (queueCountText != null)
            queueCountText.text = count > 0 ? count.ToString() : "";
    }

    // ==== Обработчики кнопок ====

    private void OnAddToQueue()
    {
        building.EnqueueCurrentRecipe();
    }

    private void OnClearQueue()
    {
        craftingSystem.ClearQueue();
        craftingSystem.OnQueueCountChanged(0);
    }
}
