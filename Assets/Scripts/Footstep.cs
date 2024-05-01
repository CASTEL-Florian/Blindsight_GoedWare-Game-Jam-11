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
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            float alpha = alphaCurve.Evaluate((Time.time - startTime) / lifetime);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
