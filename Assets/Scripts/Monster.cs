using UnityEngine;
using UnityEngine.AI;



public class Monster : MonoBehaviour
{
    public enum MonsterBehaviour
    {
        Idle,
        Chase,
    }
    
    
    private bool isDeactivated = false;
    private Player player;
    private NavMeshAgent navMeshAgent;
    private Vector3 target;
    private MonsterBehaviour currentBehaviour = MonsterBehaviour.Idle;
    
    [SerializeField] private int soundRayCount = 5;
    [SerializeField] private float soundRaySpeed = 2f;
    [SerializeField] private float soundRayLifetime = 1f;
    
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private float distanceFromCenter = 0.2f;
    [SerializeField] private float distanceBetweenSoundEmission = 0.5f;
    private Coroutine audioFadeCoroutine;
    private float targetStartTime;
    private Vector3 lastSoundPosition;
    
    private void Start()
    {
        player = FindObjectOfType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        audioSource.volume = 0.0f;
        lastSoundPosition = transform.position;
    }
    
    private void Update()
    {
        if (currentBehaviour == MonsterBehaviour.Idle) return;
        if (isDeactivated)
        {
            navMeshAgent.SetDestination(transform.position);
            return;
        }
        navMeshAgent.SetDestination(target);
        if (Vector2.Distance(transform.position, lastSoundPosition) > distanceBetweenSoundEmission)
        {
            float angle = Random.Range(0, 360);
            SoundEmitter.Instance.EmitSound(transform.position, soundRayCount, soundRaySpeed, soundRayLifetime, SoundEmitter.SoundType.MonsterWalk, angle, distanceFromCenter);
            lastSoundPosition = transform.position;
        }
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentBehaviour = MonsterBehaviour.Idle;
            if (audioFadeCoroutine != null)
            {
                StopCoroutine(audioFadeCoroutine);
            }
            audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioSource, 1f, 0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDeactivated) return;
        if (other.CompareTag("SoundRay"))
        {
            SoundRay soundRay = other.GetComponent<SoundRay>();
            ReactToSound(soundRay);
        } 
    }

    private void ReactToSound(SoundRay soundRay)
    {
        if (soundRay.RaySoundType == SoundEmitter.SoundType.PlayerWalk || soundRay.RaySoundType == SoundEmitter.SoundType.PlayerClap)
        {
            if (currentBehaviour == MonsterBehaviour.Chase)
            {
                if (soundRay.StartTime < targetStartTime)
                {
                    return;
                }
                target = soundRay.Origin;
                targetStartTime = soundRay.StartTime;
                return;
            }
            currentBehaviour = MonsterBehaviour.Chase;
            target = soundRay.Origin;
            targetStartTime = soundRay.StartTime;
            audioSource.Play();
            if (audioFadeCoroutine != null)
            {
                StopCoroutine(audioFadeCoroutine);
            }
            audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioSource, 0.5f, 1f));
        }
    }
    
    public void Deactivate()
    {
        isDeactivated = true;
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioSource, 0.3f, 0f));
    }
}
