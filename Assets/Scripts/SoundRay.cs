using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRay : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<TrailRenderer> trailRenderer;
    [SerializeField] private AnimationCurve alphaCurve;

    public void Init(float speed, float lifetime)
    {
        rb.velocity = transform.right * speed;
        StartCoroutine(FadeOut(lifetime));
    }
    
    private IEnumerator FadeOut(float lifetime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            float alpha = alphaCurve.Evaluate((Time.time - startTime) / lifetime);
            foreach (var renderer in trailRenderer)
            {
                renderer.material.color = new Color(1, 1, 1, alpha);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
