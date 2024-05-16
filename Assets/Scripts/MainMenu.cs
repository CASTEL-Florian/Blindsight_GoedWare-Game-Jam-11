using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private float timeBeforeFade = 0.5f;
    [SerializeField] private List<Image> cursors;
    [SerializeField] private List<RectTransform> levelSelectPositions;
    [SerializeField] private List<int> sceneIndices;
    
    [SerializeField] private Vector3 checkpointPosition;
    [SerializeField] private float checkpointRotation;
    
    [SerializeField] private Vector3 level11CheckpointPosition;
    [SerializeField] private float level11checkpointRotation;
    
    [SerializeField] private GameObject quitButton;

    [SerializeField] private GameObject level11Button;
    
    private int currentSelectedLevel = -1;
    
    
    
    private void Start()
    {
        MusicPlayer.Instance.SceneLoaded();
        
        // Only show the quit button in Windows builds.
        if (quitButton != null && Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }
        
        StartCoroutine(FadeInCoroutine());
        if (levelSelectPositions.Count > 0)
        {
            foreach (Image cursor in cursors)
            {
                cursor.enabled = false;
            }
            cursors[0].enabled = true;
            currentSelectedLevel = 0;
        }

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.IsLastLevelBeaten)
        {
            level11Button.SetActive(true);
        }
    }
    
    private IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeFade);
        fader.FadeIn();
    }
    
    public void StartGame()
    {
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void LoadSelectedLevel()
    {
        if (currentSelectedLevel == -1) return;
        if (currentSelectedLevel == 10)
        {
            CheckpointManager.Instance.SetCheckpoint(checkpointPosition, checkpointRotation);
        }
        if (currentSelectedLevel == 12)
        {
            CheckpointManager.Instance.SetCheckpoint(level11CheckpointPosition, level11checkpointRotation);
        }
        fader.TransitionToScene(sceneIndices[currentSelectedLevel]);
    }

    public void LoadLevel11()
    {
        fader.TransitionToScene(12);
    }

    public void SelectLevel(int level)
    {
        if (currentSelectedLevel == level) return;
        cursors[currentSelectedLevel].enabled = false;
        currentSelectedLevel = level;
        cursors[currentSelectedLevel].enabled = true;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
