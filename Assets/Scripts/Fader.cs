using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] public float fadeInTime;
    [SerializeField] public float fadeOutTime;
    [SerializeField] private bool startHidden;
    public bool Transitioning { get; private set; }
    public bool fading { get; private set; }
    private Coroutine routine;


    private void Start()
    {
        if (startHidden)
        {
            Color startColor = image.color;
            startColor.a = 1;
            image.color = startColor;
        }
    }

    public void FadeOut()
    {
        if (Transitioning)
            return;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeOutRoutine(fadeOutTime));
    }
    public void FadeIn()
    {
        if (Transitioning)
            return;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeInRoutine(fadeInTime));
    }
    private IEnumerator FadeOutRoutine(float time)
    {
        fading = true;
        Color startColor = image.color;
        startColor.a = 0;
        image.color = startColor;
        while (image.color.a < 1)
        {
            Color imageColor = image.color;
            imageColor.a += Time.unscaledDeltaTime / time;
            image.color = imageColor;
            yield return null;
        }
        fading = false;
    }

    private IEnumerator FadeInRoutine(float time)
    {
        fading = true;
        Color startColor = image.color;
        startColor.a = 1;
        image.color = startColor;
        while (image.color.a > 0)
        {
            Color imageColor = image.color;
            imageColor.a -= Time.unscaledDeltaTime / time;
            image.color = imageColor;
            yield return null;
        }
        fading = false;
    }

    public void TransitionToScene(int sceneIndex, float fadeOutDuration = 1)
    {
        if (Transitioning)
            return;
        Transitioning = true;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(TransitionRoutine(sceneIndex, fadeOutDuration));
    }
    private IEnumerator TransitionRoutine(int sceneIndex, float fadeOutDuration)
    {
        yield return FadeOutRoutine(fadeOutDuration);
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneIndex);
    }
}
