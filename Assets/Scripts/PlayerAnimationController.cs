using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string interactParam = "IsInteracting";

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField][Range(0.8f, 1.2f)] private float minPitch = 0.9f;
    [SerializeField][Range(0.8f, 1.2f)] private float maxPitch = 1.1f;
    [SerializeField][Range(0.1f, 1f)] private float volume = 0.5f;

    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    //Обновить скорость в анимации
    public void UpdateSpeed(float speed)
    {
        animator.SetFloat(speedParam, speed);
    }

    ///Включить/выключить анимацию взаимодействия
    public void PlayInteract(bool state)
    {
        animator.SetBool(interactParam, state);
    }

    ///Проиграть звук шага (вызывается через Animation Event)
    public void OnFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip, volume);
    }
}
