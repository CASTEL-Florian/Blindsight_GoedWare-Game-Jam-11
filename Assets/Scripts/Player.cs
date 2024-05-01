using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private SoundEmitter soundEmitter;
    
    [SerializeField] private float clapDirectionCount = 20;
    [SerializeField] private float clapRaySpeed = 7f;
    [SerializeField] private float clapRayLifetime = 3f;
    [SerializeField] private float clapCooldown = 0.5f;
    
    private PlayerController controller;
    private float currentClapCooldown = 0f;
    
    public bool Alive { get; private set; } = true;
    private void Awake()
    {
        controller = new PlayerController();
    }

    private void OnEnable()
    {
        controller.Player.Enable();
        controller.Player.Clap.performed += Clap;
    }
    
    private void OnDisable()
    {
        controller.Player.Disable();
        controller.Player.Clap.performed -= Clap;
    }

    private void Update()
    {
        if (!Alive) return;
        Vector2 movement = controller.Player.Move.ReadValue<Vector2>();
        bool running = controller.Player.Run.ReadValue<float>() > 0.5f;
        bool sneaking = controller.Player.Sneak.ReadValue<float>() > 0.5f;
        playerMovement.Move(movement, running, sneaking);
        
        if (currentClapCooldown > 0)
        {
            currentClapCooldown -= Time.deltaTime;
        }
    }

    private void Clap(InputAction.CallbackContext ctx)
    {
        if (!Alive || currentClapCooldown > 0) return;
        playerAudio.PlayClapSound();
        float angleOffset = Random.Range(-180, 180);
        soundEmitter.EmitSound(transform.position, (int) clapDirectionCount, clapRaySpeed, clapRayLifetime, SoundEmitter.SoundType.Normal, angleOffset);
        currentClapCooldown = clapCooldown;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Monster"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (!Alive) return;
        playerAudio.PlayDeathSound();
        soundEmitter.EmitSound(transform.position, 50, 8, 3, SoundEmitter.SoundType.Death);
        CameraShake.Instance.ShakeCamera(0.2f);
        Alive = false;
        StartCoroutine(RestartCoroutine());
    }
    
    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.Restart();
    }
}
