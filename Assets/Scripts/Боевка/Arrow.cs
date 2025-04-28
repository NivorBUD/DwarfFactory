using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;
    private IDamageSource shooter;
    private bool hasHit;

    public void Init(IDamageSource src)
    {
        shooter = src;
        Destroy(gameObject, lifeTime);
    }

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || shooter == null) return;
        var h = other.GetComponent<HealthSystem>();
        if (h != null && h.myType != shooter.GetSourceType() && !h.IsDead())
        {
            hasHit = true;
            h.TakeDamage(shooter.GetDamage());
            Destroy(gameObject);
        }
    }
}
