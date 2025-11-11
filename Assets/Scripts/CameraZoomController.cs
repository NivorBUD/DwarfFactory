using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private CinemachineCamera virtualCamera; // ������ �� Cinemachine 2D Camera
    [SerializeField] private float minZoom = 4f; // ����������� ��������������� ������
    [SerializeField] private float maxZoom = 6f; // ������������ ��������������� ������
    [SerializeField] private float zoomSensitivity = 0.2f; // ���������������� ��� ����
    [SerializeField] private float zoomSmoothness = 10f; // �������� ����������� ����

    private InputSystem_Actions controls;
    private float targetOrthoSize;
    private float currentOrthoSize;

    private void Awake()
    {
        // ������������� Input System
        controls = new InputSystem_Actions();

        // �������� CinemachineCamera
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineCamera not assigned in Inspector!");
            return;
        }

        // ������������� ���������� ����
        targetOrthoSize = virtualCamera.Lens.OrthographicSize;
        currentOrthoSize = targetOrthoSize;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        if (virtualCamera == null) return;

        bool zoomChanged = false;

        // Обработка зума на клавиши - и +
        if (Keyboard.current != null)
        {
            // Зум OUT (отдалить) - клавиша Minus
            if (Keyboard.current.minusKey.isPressed || Keyboard.current.numpadMinusKey.isPressed)
            {
                targetOrthoSize = Mathf.Clamp(targetOrthoSize + zoomSensitivity * Time.deltaTime * 10f, minZoom, maxZoom);
                zoomChanged = true;
            }
            
            // Зум IN (приблизить) - клавиша Plus (Equals без Shift)
            if (Keyboard.current.equalsKey.isPressed || Keyboard.current.numpadPlusKey.isPressed)
            {
                targetOrthoSize = Mathf.Clamp(targetOrthoSize - zoomSensitivity * Time.deltaTime * 10f, minZoom, maxZoom);
                zoomChanged = true;
            }
        }

        // Плавное приближение к целевому зуму
        if (zoomChanged || Mathf.Abs(currentOrthoSize - targetOrthoSize) > 0.001f)
        {
            currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, zoomSmoothness * Time.deltaTime);

            // Применяем к камере ТОЛЬКО если изменилось
            var lens = virtualCamera.Lens;
            lens.OrthographicSize = currentOrthoSize;
            virtualCamera.Lens = lens;
        }
    }
}