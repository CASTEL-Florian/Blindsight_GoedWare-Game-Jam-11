using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float sneakSpeed = 1f;
    [SerializeField] private float smoothRotation = 0.3f;
    
    [SerializeField] private GameObject leftFootPrefab;
    [SerializeField] private GameObject rightFootPrefab;
    [SerializeField] private float stepDistance = 0.5f;  // Distance to move before spawning the next footstep
    [SerializeField] private float footOffset = 0.2f;    // Horizontal offset of the footsteps from the center
    
    [SerializeField] private SoundEmitter soundEmitter;
    
    [SerializeField] private int directionCount = 10;
    [SerializeField] private float raySpeed = 5f;
    [SerializeField] private float rayLifetime = 2f;
    
    [SerializeField] private int runDirectionCount = 15;
    [SerializeField] private float runRaySpeed = 6f;
    [SerializeField] private float runRayLifetime = 2f;
    
    [SerializeField] private int sneakDirectionCount = 5;
    [SerializeField] private float sneakRaySpeed = 2f;
    [SerializeField] private float sneakRayLifetime = 1f;
    
    [SerializeField] private int visibleFootsteps = 7;
    
    [SerializeField] private float sneakVolume = 0.5f;
    [SerializeField] private float runVolume = 1f;
    [SerializeField] private float walkVolume = 0.75f;
    
    [SerializeField] private PlayerAudio playerAudio;
    
    
    private Vector3 lastStepPosition;
    private bool useRightFoot = true;
    private Queue<Footstep> footstepQueue = new();
    

    void Start()
    {
        lastStepPosition = transform.position;
        
        Vector3 offsetR = Quaternion.Euler(0, 0, transform.eulerAngles.z) * new Vector3(footOffset, 0, 0);
        Vector3 offsetL = Quaternion.Euler(0, 0, transform.eulerAngles.z) * new Vector3(-footOffset, 0, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        
        Footstep footstepR = Instantiate(rightFootPrefab, transform.position + offsetR, rotation).GetComponent<Footstep>();
        Footstep footstepL = Instantiate(leftFootPrefab, transform.position + offsetL, rotation).GetComponent<Footstep>();
        
        footstepQueue.Enqueue(footstepL);
        footstepQueue.Enqueue(footstepR);

    }

    
    
    public void Move(Vector2 movement, bool running, bool sneaking)
    {
        if (sneaking)
        {
            transform.position += new Vector3(movement.x, movement.y, 0) * (Time.deltaTime * sneakSpeed);
        }
        else
        {
            transform.position += new Vector3(movement.x, movement.y, 0) * (Time.deltaTime * (running ? runSpeed : walkSpeed));
        }
        
        if (movement.magnitude < 0.1f) return;
        float angle = -Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), smoothRotation * Time.deltaTime);
        
        Vector3 currentPosition = transform.position;
        float currentAngle = transform.eulerAngles.z;

        // Check if the position or rotation has changed enough to warrant a new footstep
        if (Vector3.Distance(currentPosition, lastStepPosition) > stepDistance)
        {
            SpawnFootstep(currentPosition, currentAngle, running, sneaking);
            lastStepPosition = currentPosition;
        }
    }
    
    
    private void SpawnFootstep(Vector3 position, float angle, bool running, bool sneaking)
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
        
        
        if (sneaking)
        {
            soundEmitter.EmitSound(position + offset, sneakDirectionCount, sneakRaySpeed, sneakRayLifetime, SoundEmitter.SoundType.Normal, angle);
            playerAudio.PlayFootstepSound(sneakVolume);
        }
        else if (running)
        {
            soundEmitter.EmitSound(position + offset, runDirectionCount, runRaySpeed, runRayLifetime, SoundEmitter.SoundType.Normal, angle);
            playerAudio.PlayFootstepSound(runVolume);
        }
        else
        {
            soundEmitter.EmitSound(position + offset, directionCount, raySpeed, rayLifetime, SoundEmitter.SoundType.Normal, angle);
            playerAudio.PlayFootstepSound(walkVolume);
        }
    }
}
