using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAudio playerAudio;
    
    [SerializeField] private float clapDirectionCount = 20;
    [SerializeField] private float clapRaySpeed = 7f;
    [SerializeField] private float clapRayLifetime = 3f;
    [SerializeField] private float clapCooldown = 0.5f;
    
    [SerializeField] private int enterWaterDirectionCount = 30;
    [SerializeField] private float waterRaySpeed = 5f;
    [SerializeField] private float waterRayLifetime = 3f;
    
    
    private PlayerController controller;
    private float currentClapCooldown = 0f;
    private WinTrigger[] winTriggers;
    
    public bool Alive { get; private set; } = true;
    private void Awake()
    {
        controller = new PlayerController();
    }

    private void Start()
    {
        winTriggers = FindObjectsOfType<WinTrigger>();
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
        if (!Alive)
        {
            playerMovement.Move(Vector2.zero, false);
            return;
        }
        Vector2 movement = controller.Player.Move.ReadValue<Vector2>();
        bool sneaking = controller.Player.Sneak.ReadValue<float>() > 0.5f;
        playerMovement.Move(movement, sneaking);
        
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
        SoundEmitter.Instance.EmitSound(transform.position, (int) clapDirectionCount, clapRaySpeed, clapRayLifetime, SoundEmitter.SoundType.PlayerClap, angleOffset);
        currentClapCooldown = clapCooldown;
    }

    public void TriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Monster"))
        {
            Die();
        }

        if (other.CompareTag("Water"))
        {
            if (playerMovement.InWater) return;
            playerAudio.PlayEnterWaterSound();
            playerMovement.InWater = true;
            SoundEmitter.Instance.EmitSound(transform.position, enterWaterDirectionCount, waterRaySpeed, waterRayLifetime, SoundEmitter.SoundType.PlayerWalk);
        }
        
        if (other.CompareTag("WinTrigger"))
        {
            other.GetComponent<WinTrigger>().Activate();
        }
    }
    
    public void TriggerExit(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            playerMovement.InWater = false;
        }
    }

    private void Die()
    {
        if (!Alive) return;
        playerAudio.PlayDeathSound();
        SoundEmitter.Instance.EmitSound(transform.position, 50, 8, 3, SoundEmitter.SoundType.Death);
        MyCamera.Instance.ShakeCamera(0.2f);
        Alive = false;
        foreach (WinTrigger winTrigger in winTriggers)
        {
            winTrigger.enabled = false;
        }
        StartCoroutine(RestartCoroutine());
    }
    
    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.Restart();
    }

    public void Win()
    {
        if (!Alive) return;
        Alive = false;
    }
}
