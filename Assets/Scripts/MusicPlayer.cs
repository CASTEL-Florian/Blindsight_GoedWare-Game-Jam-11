using UnityEngine;
using UnityEngine.Audio;


// A class that handles the background music in the game. It is not destroyed when a new scene is loaded.
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float fadeInTime = 1f;
    
    Coroutine audioFadeCoroutine;

    private bool prepareFadeIn = false;
    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(gameObject); }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioMixer.SetFloat("MusicVolume", -80);
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeInTime, 1f));
    }

    public void FadeOut(float fadeOutTime)
    {
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeOutTime, 0.0001f));
    }
    
    public void FadeIn()
    {
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeInTime, 1f));
    }

    public void PrepareFadeInAtNextScene()
    {
        prepareFadeIn = true;
    }

    public void SceneLoaded()
    {
        if (prepareFadeIn)
        {
            FadeIn();
            prepareFadeIn = false;
        }
    }
}
