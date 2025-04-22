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

    private InputSystem_Actions controls; // ����� ����������
    private float zoomInput; // ���� ���� �� ����
    private float targetOrthoSize; // ������� ������ ����
    private float currentOrthoSize; // ������� ������ ����

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
        // �������� �� ���� ����
        controls.Player.Zoom.performed += OnZoom;
        controls.Player.Zoom.canceled += OnZoom;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        // ������� �� �����
        controls.Player.Zoom.performed -= OnZoom;
        controls.Player.Zoom.canceled -= OnZoom;
        controls.Player.Disable();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        // ��������� �������� ���� �� ������ ���� (scroll.y: ������������� ��� ��������� �����, ������������� ��� ����)
        Vector2 input = context.ReadValue<Vector2>();
        zoomInput = input.y;

    }

    private void Update()
    {
        if (virtualCamera == null) return;

        // ��������� ����� ���� �� ������ ����
        if (zoomInput != 0f)
        {
            // ��������� ��������� ���� (��������� ����� = ���������� orthoSize, ���� = ����������)
            float zoomDelta = -zoomInput * zoomSensitivity;
            targetOrthoSize = Mathf.Clamp(targetOrthoSize + zoomDelta, minZoom, maxZoom);
        }

        // ������� ����������� ����
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, zoomSmoothness * Time.deltaTime);

        // ���������� Lens
        var lens = virtualCamera.Lens;
        lens.OrthographicSize = currentOrthoSize;
        virtualCamera.Lens = lens;
    }
}