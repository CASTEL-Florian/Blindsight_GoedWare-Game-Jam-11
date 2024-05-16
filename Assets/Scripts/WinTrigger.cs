using System.Collections;
using UnityEngine;

// Trigger that activates the win state of the game.
public class WinTrigger : MonoBehaviour
{
    [SerializeField] private float raySpeed = 3f;
    [SerializeField] private float rayLifetime = 6f;
    [SerializeField] private int rayDirectionCount = 50;
    [SerializeField] private float distanceFromCenter = 0.5f;
    [SerializeField] private float widthMultiplier = 1.5f;
    
    [SerializeField] private AudioSource audioSource;
    private bool isDeactivated = false;
    
    public void Activate()
    {
        if (isDeactivated) return;

        StartCoroutine(EmissionCoroutine());
        audioSource.Play();
        isDeactivated = true;
        GameManager.Instance.Win();
    }

    private IEnumerator EmissionCoroutine()
    {
        SoundEmitter.Instance.EmitSound(transform.position, rayDirectionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Win, 0, distanceFromCenter, rayColor:SoundEmitter.RayColor.Yellow, widthMultiplier:widthMultiplier);
        SoundEmitter.Instance.EmitSound(transform.position, rayDirectionCount, raySpeed * 0.625f, rayLifetime, SoundEmitter.SoundType.Win, 0, distanceFromCenter, 0.5f, rayColor:SoundEmitter.RayColor.Yellow, widthMultiplier:widthMultiplier);
        SoundEmitter.Instance.EmitSound(transform.position, rayDirectionCount, raySpeed/4, rayLifetime, SoundEmitter.SoundType.Win, 0, distanceFromCenter, 0.2f, rayColor:SoundEmitter.RayColor.Yellow, widthMultiplier:widthMultiplier);
        yield return null;
    }
}
