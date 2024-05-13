using UnityEngine;

// Trigger that activates the win state of the game.
public class WinTrigger : MonoBehaviour
{
    [SerializeField] private float raySpeed = 3f;
    [SerializeField] private float rayLifetime = 6f;
    [SerializeField] private int rayDirectionCount = 50;
    [SerializeField] private float distanceFromCenter = 0.5f;
    
    [SerializeField] private AudioSource audioSource;
    private bool isDeactivated = false;
    
    public void Activate()
    {
        if (isDeactivated) return;

        SoundEmitter.Instance.EmitSound(transform.position, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Win, 0, distanceFromCenter);
        audioSource.Play();
        isDeactivated = true;
        GameManager.Instance.Win();
    }
}
