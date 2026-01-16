using System.Linq;
using UnityEngine;

public class AllowedTypeSlot : InventorySlot
{
    [SerializeField] private ItemType allowedType;

    private static AudioSource audioSource;
    private static AudioClip takeSwordSound;
    private static AudioClip wearArmorSound;

    private void Start()
    {
        // Инициализируем статические звуки только один раз
        if (audioSource == null)
        {
            // Ищем AudioSource на объекте InventoryManager или создаем новый
            var manager = FindObjectOfType<InventoryManager>();
            if (manager != null)
            {
                audioSource = manager.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = manager.gameObject.AddComponent<AudioSource>();
                }
            }

            // Загружаем звуки из Resources
            takeSwordSound = Resources.Load<AudioClip>("Sounds/TakeSword");
            wearArmorSound = Resources.Load<AudioClip>("Sounds/WearArmor");
        }
    }

    public override void Set(ItemScriptableObject item, int amount = 1)
    {
        if (item == null || IsAllowed(item))
        {
            // Проигрываем звук только если добавляем предмет (не убираем)
            if (item != null && Item == null)
            {
                PlayEquipSound(item.itemType);
            }
            
            base.Set(item, amount);
        }
    }

    public override int AddAmount(int amount)
    {
        if (Item != null && IsAllowed(Item))
            return base.AddAmount(amount);

        return 0;
    }

    public bool IsAllowed(ItemScriptableObject item)
        => allowedType == item.itemType;

    public void SetAllowedType(ItemType type)
    {
        allowedType = type;
    }

    private void PlayEquipSound(ItemType itemType)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = null;

        if (itemType == ItemType.Weapon)
        {
            clipToPlay = takeSwordSound;
        }
        else if (itemType == ItemType.Helmet || itemType == ItemType.Chestplate || itemType == ItemType.Boots)
        {
            clipToPlay = wearArmorSound;
        }

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}
