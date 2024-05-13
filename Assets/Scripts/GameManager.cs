using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Fader fader;
    [SerializeField] private float timeBeforeNextLevel = 2f;
    
    
    
    [SerializeField] private bool lastLevel;
    [SerializeField] private SpriteRenderer hideSurface;
    [SerializeField] private Transform endScreen;
    [SerializeField] private GameObject titleSprite;
    [SerializeField] private AudioSource endAudioSource;
    
    private NavMeshSurface[] navSurfaces;
    private List<Monster> monsters = new();
    private Player player;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        navSurfaces = FindObjectsOfType<NavMeshSurface>();
        foreach (var navSurface in navSurfaces)
        {
            navSurface.BuildNavMesh();
        }
        
        monsters.AddRange(FindObjectsOfType<Monster>());
        player = FindObjectOfType<Player>();
        fader.FadeIn();
    }
    
    public void Restart()
    {
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void UpdateNavMeshNextFrame()
    {
        StartCoroutine(UpdateNavMeshCoroutine());
    }
    
    private IEnumerator UpdateNavMeshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        foreach (var navSurface in navSurfaces)
        {
            navSurface.UpdateNavMesh(navSurface.navMeshData);
        }
    }

    public void Win()
    {
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.ResetCheckpoint();
        }
        DeactivateMonsters();
        player.Win();
        if (!lastLevel)
        {
            StartCoroutine(LoadNextLevelCoroutine());
            return;
        }
        
        Vector3 cameraPosition = MyCamera.Instance.transform.position;
        endScreen.gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        StartCoroutine(EndCoroutine());
    }

    private IEnumerator EndCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeNextLevel);

        float alpha = 0.5f;
        Color color = hideSurface.color;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            color.a = alpha;
            hideSurface.color = color;
            yield return null;
        }
        MyCamera.Instance.SnapToDesiredPosition();
        Vector3 cameraPosition = MyCamera.Instance.transform.position;
        endScreen.gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        yield return new WaitForSeconds(3f);
        
        titleSprite.gameObject.SetActive(true);
        endAudioSource.Play();
        SoundEmitter.Instance.EmitSound(titleSprite.transform.position, 50, 5, 5, SoundEmitter.SoundType.Ending, 0, 3);
        yield return new WaitForSeconds(4f);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        MusicPlayer.Instance.FadeOut(3f);
        MusicPlayer.Instance.PrepareFadeInAtNextScene();
        fader.TransitionToScene(nextSceneIndex, 3f);
    }
    
    
    public void DeactivateMonsters()
    {
        foreach (var monster in monsters)
        {
            monster.Deactivate();
        }
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeNextLevel);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        fader.TransitionToScene(nextSceneIndex);
    }
}
