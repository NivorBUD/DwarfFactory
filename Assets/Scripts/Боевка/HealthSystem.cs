using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("��������� ��������")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("��� �������������� � �����")]
    public TargetType myType;        // � ���� ���������
    public TargetType targetType;    // ���� �������

    [Header("UI")]
    public GameObject healthBarPrefab; // ������ ������ ��������

    private HealthBar healthBar;      // ��������� ������ ��������

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject hb = Instantiate(healthBarPrefab, canvas.transform);
                hb.name = $"{gameObject.name}_HealthBar";
                healthBar = hb.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.SetTarget(transform, this);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void Die()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

public enum TargetType
{
    Player,
    AllyBot,
    EnemyBot
}