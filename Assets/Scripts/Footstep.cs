using System.Collections;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void FadeOut(float lifetime)
    {
        StartCoroutine(FadeOutCoroutine(lifetime));
    }
    
    private IEnumerator FadeOutCoroutine(float lifetime)
    {
        Color startColor = spriteRenderer.color;
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            float alpha = alphaCurve.Evaluate((Time.time - startTime) / lifetime);
            Color color = startColor;
            color.a = alpha;
            spriteRenderer.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
