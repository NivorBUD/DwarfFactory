using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private CinemachineCamera virtualCamera; // Ссылка на Cinemachine 2D Camera
    [SerializeField] private float minZoom = 4f; // Минимальный ортографический размер
    [SerializeField] private float maxZoom = 6f; // Максимальный ортографический размер
    [SerializeField] private float zoomSensitivity = 0.2f; // Чувствительность для мыши
    [SerializeField] private float zoomSmoothness = 10f; // Скорость сглаживания зума

    private InputSystem_Actions controls; // Карта управления
    private float zoomInput; // Ввод зума от мыши
    private float targetOrthoSize; // Целевой размер зума
    private float currentOrthoSize; // Текущий размер зума

    private void Awake()
    {
        // Инициализация Input System
        controls = new InputSystem_Actions();

        // Проверка CinemachineCamera
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineCamera not assigned in Inspector!");
            return;
        }

        // Инициализация начального зума
        targetOrthoSize = virtualCamera.Lens.OrthographicSize;
        currentOrthoSize = targetOrthoSize;
    }

    private void OnEnable()
    {
        // Подписка на ввод зума
        controls.Player.Zoom.performed += OnZoom;
        controls.Player.Zoom.canceled += OnZoom;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        // Отписка от ввода
        controls.Player.Zoom.performed -= OnZoom;
        controls.Player.Zoom.canceled -= OnZoom;
        controls.Player.Disable();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        // Получение значения зума от колеса мыши (scroll.y: положительное для прокрутки вверх, отрицательное для вниз)
        Vector2 input = context.ReadValue<Vector2>();
        zoomInput = input.y;

    }

    private void Update()
    {
        if (virtualCamera == null) return;

        // Обработка ввода зума от колеса мыши
        if (zoomInput != 0f)
        {
            // Вычисляем изменение зума (прокрутка вверх = уменьшение orthoSize, вниз = увеличение)
            float zoomDelta = -zoomInput * zoomSensitivity;
            targetOrthoSize = Mathf.Clamp(targetOrthoSize + zoomDelta, minZoom, maxZoom);
        }

        // Плавное сглаживание зума
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, zoomSmoothness * Time.deltaTime);

        // Обновление Lens
        var lens = virtualCamera.Lens;
        lens.OrthographicSize = currentOrthoSize;
        virtualCamera.Lens = lens;
    }
}