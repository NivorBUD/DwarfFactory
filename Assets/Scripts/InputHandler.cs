using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Централизованный обработчик всех игровых нажатий клавиш.
/// Использует Unity Input System для обработки ввода и делегирует действия соответствующим менеджерам.
/// </summary>
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Input Actions")]
    private InputSystem_Actions controls;

    // События для подписки менеджеров
    public event Action OnInventoryToggle;
    public event Action OnPauseToggle;
    public event Action OnBuildingPlace;
    public event Action<int> OnQuickSlotSelect; // 1-9
    public event Action OnScrollUp;
    public event Action OnScrollDown;
    public event Action OnBuildingInteract;
    public event Action OnChestInteract;

    // Флаги состояния для модификаторов
    public bool IsShiftHeld { get; private set; }
    public bool IsControlHeld { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Player actions уже обрабатываются в других скриптах (Move, Interact, Zoom)
        // Здесь подписываемся только на UI и дополнительные действия
        controls.UI.Cancel.performed += OnCancelPerformed;
        controls.Player.Enable();
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Cancel.performed -= OnCancelPerformed;
        controls.Player.Disable();
        controls.UI.Disable();
    }

    private void Update()
    {
        // Обработка клавиш, которые пока не в Input System
        HandleLegacyInput();
    }

    /// <summary>
    /// Временная обработка старых Input вызовов до полного перехода на Input System
    /// </summary>
    private void HandleLegacyInput()
    {
        // Инвентарь (E)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnInventoryToggle?.Invoke();
        }

        // Пауза (Escape)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnPauseToggle?.Invoke();
        }

        // Размещение здания (F)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            OnBuildingPlace?.Invoke();
        }

        // Быстрые слоты (1-9)
        if (Keyboard.current != null)
        {
            for (int i = 1; i <= 9; i++)
            {
                var key = Keyboard.current[(Key)(i + (int)Key.Digit1 - 1)];
                if (key.wasPressedThisFrame)
                {
                    OnQuickSlotSelect?.Invoke(i);
                    break;
                }
            }
        }

        // Скролл колесика мыши для хотбара (только когда инвентарь закрыт И не строим здание)
        if (Mouse.current != null && 
            InventoryManager.Instance != null && 
            !InventoryManager.Instance.ui.IsInventoryOpened &&
            BuildingsGrid.Instance != null &&
            !BuildingsGrid.Instance.IsPlacingBuilding)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll > 0.1f)
            {
                OnScrollUp?.Invoke();
            }
            else if (scroll < -0.1f)
            {
                OnScrollDown?.Invoke();
            }
        }

        // Взаимодействие с объектами (ПКМ)
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnBuildingInteract?.Invoke();
            OnChestInteract?.Invoke();
        }

        // Размещение здания (ЛКМ)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Это обрабатывается в BuildingsGrid
        }

        // Модификаторы
        if (Keyboard.current != null)
        {
            IsShiftHeld = Keyboard.current.leftShiftKey.isPressed;
            IsControlHeld = Keyboard.current.leftCtrlKey.isPressed;
        }
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        OnPauseToggle?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
