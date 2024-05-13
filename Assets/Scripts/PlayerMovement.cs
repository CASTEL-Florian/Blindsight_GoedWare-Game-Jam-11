using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float sneakSpeed = 1f;
    [SerializeField] private float smoothRotation = 0.3f;
    
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;
    [SerializeField] private float stepDistance = 0.5f;  // Distance to move before spawning the next footstep
    [SerializeField] private float footOffset = 0.2f;    // Horizontal offset of the footsteps from the center
    
    [SerializeField] private int runDirectionCount = 15;
    [SerializeField] private float runRaySpeed = 6f;
    [SerializeField] private float runRayLifetime = 2f;
    
    [SerializeField] private int sneakDirectionCount = 5;
    [SerializeField] private float sneakRaySpeed = 2f;
    [SerializeField] private float sneakRayLifetime = 1f;
    
    [SerializeField] private int visibleFootsteps = 7;
    
    [SerializeField] private float sneakVolume = 0.5f;
    [SerializeField] private float runVolume = 1f;
    [SerializeField] private float waterVolume = 0.75f;
    
    [SerializeField] private float waterSpeedMultiplier = 0.7f;
    [SerializeField] private int waterDirectionCount = 15;
    [SerializeField] private float waterRaySpeed = 1.5f;
    [SerializeField] private float waterRayLifetime = 3f;
    
    [SerializeField] private float sneakStartAlpha = 0.5f;
    
    [SerializeField] private PlayerAudio playerAudio;
    
    [SerializeField] private Rigidbody2D rb;
    
    [SerializeField] private MyCamera myCamera;
    
    
    private Vector3 lastStepPosition;
    private bool useRightFoot = true;
    private Queue<Footstep> footstepQueue = new();
    
    public bool InWater { get; set; } = false;
    

    void Start()
    {
        if (CheckpointManager.Instance != null && CheckpointManager.Instance.CheckpointSet)
        {
            transform.position = CheckpointManager.Instance.LastCheckpointPosition;
            transform.eulerAngles = new Vector3(0, 0, CheckpointManager.Instance.LastCheckpointRotation);
            myCamera.transform.position = transform.position;
        }
        lastStepPosition = transform.position;
        
        Vector3 offsetR = Quaternion.Euler(0, 0, transform.eulerAngles.z) * new Vector3(footOffset, 0, 0);
        Vector3 offsetL = Quaternion.Euler(0, 0, transform.eulerAngles.z) * new Vector3(-footOffset, 0, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        
        Footstep footstepR = Instantiate(rightFootPrefab, transform.position + offsetR, rotation).GetComponent<Footstep>();
        Footstep footstepL = Instantiate(leftFootPrefab, transform.position + offsetL, rotation).GetComponent<Footstep>();
        
        footstepQueue.Enqueue(footstepL);
        footstepQueue.Enqueue(footstepR);

    }

    
    
    public void Move(Vector2 movement, bool sneaking)
    {
        if (sneaking)
        {
            rb.velocity = movement * (sneakSpeed * (InWater ? waterSpeedMultiplier : 1f));
        }
        else
        {
            rb.velocity = movement * runSpeed * (InWater ? waterSpeedMultiplier : 1f);
        }
        
        if (movement.magnitude < 0.1f) return;
        float angle = -Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), smoothRotation * Time.deltaTime);
        
        Vector3 currentPosition = transform.position;
        float currentAngle = transform.eulerAngles.z;

        // Check if the position or rotation has changed enough to warrant a new footstep
        if (Vector3.Distance(currentPosition, lastStepPosition) > stepDistance)
        {
            SpawnFootstep(currentPosition, currentAngle, sneaking);
            lastStepPosition = currentPosition;
        }
    }
    
    
    private void SpawnFootstep(Vector3 position, float angle, bool sneaking)
    {
        GameObject footPrefab = useRightFoot ? rightFootPrefab : leftFootPrefab;
        Vector3 offset = Quaternion.Euler(0, 0, angle) * new Vector3(footOffset * (useRightFoot ? 1 : -1), 0, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        Footstep footstep = Instantiate(footPrefab, position + offset, rotation).GetComponent<Footstep>();
        footstepQueue.Enqueue(footstep);
        if (footstepQueue.Count > visibleFootsteps)
        {
            Footstep oldestFootstep = footstepQueue.Dequeue();
            oldestFootstep.FadeOut(2f);
        }
        
        useRightFoot = !useRightFoot;
        
        if (InWater)
        {
            SoundEmitter.Instance.EmitSound(position + offset, waterDirectionCount, waterRaySpeed, waterRayLifetime, SoundEmitter.SoundType.PlayerWalk, angle);
            playerAudio.PlayWaterFootstepSound(waterVolume);
        }
        else if (sneaking)
        {
            SoundEmitter.Instance.EmitSound(position + offset, sneakDirectionCount, sneakRaySpeed, sneakRayLifetime, SoundEmitter.SoundType.PlayerSneak, angle, 0, sneakStartAlpha);
            playerAudio.PlayFootstepSound(sneakVolume);
        }
        else
        {
            SoundEmitter.Instance.EmitSound(position + offset, runDirectionCount, runRaySpeed, runRayLifetime, SoundEmitter.SoundType.PlayerWalk, angle);
            playerAudio.PlayFootstepSound(runVolume);
        }
    }
}
