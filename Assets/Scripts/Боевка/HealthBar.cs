using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("UI Компоненты")]
    public Slider slider;         // Компонент Slider для управления заливкой
    public TextMeshProUGUI healthText; // Текст здоровья
    public float offsetY = 50f;   // Смещение над ботом в пикселях

    private Transform target;     // Цель (бот)
    private HealthSystem healthSystem; // Система здоровья
    private RectTransform rectTransform; // RectTransform для позиционирования

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
            Destroy(gameObject); // Уничтожаем HealthBar, если healthSystem null
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
        // Уничтожаем HealthBar, если объект всё ещё существует
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}