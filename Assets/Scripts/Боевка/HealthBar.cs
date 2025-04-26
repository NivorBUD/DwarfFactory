using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("UI ����������")]
    public Slider slider;         // ��������� Slider ��� ���������� ��������
    public TextMeshProUGUI healthText; // ����� ��������
    public float offsetY = 50f;   // �������� ��� ����� � ��������

    private Transform target;     // ���� (���)
    private HealthSystem healthSystem; // ������� ��������
    private RectTransform rectTransform; // RectTransform ��� ����������������

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
    }

    public void SetTarget(Transform targetTransform, HealthSystem health)
    {
        target = targetTransform;
        healthSystem = health;

        if (healthSystem != null && slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = healthSystem.GetMaxHealth();
            slider.value = healthSystem.GetCurrentHealth();
            UpdateHealthUI();
        }
    }

    private void LateUpdate()
    {
        if (target != null && Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            rectTransform.position = screenPos + new Vector3(0, offsetY, 0);
        }
    }

    private void Update()
    {
        if (healthSystem != null)
        {
            UpdateHealthUI();
        }
        else
        {
            Destroy(gameObject); // ���������� HealthBar, ���� healthSystem null
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSystem == null)
        {
            Destroy(gameObject);
            return;
        }

        if (slider != null)
        {
            slider.value = healthSystem.GetCurrentHealth();
        }

        if (healthText != null)
        {
            healthText.text = $"{healthSystem.GetCurrentHealth()}/{healthSystem.GetMaxHealth()}";
        }
    }

    private void OnDestroy()
    {
        // ���������� HealthBar, ���� ������ �� ��� ����������
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}