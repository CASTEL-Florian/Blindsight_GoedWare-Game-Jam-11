using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveOutline : MonoBehaviour
{
    [SerializeField] private List<LineRenderer> lineRenderers;
    [SerializeField] private AnimationCurve alphaCurve;
    
    private List<Transform> points = new List<Transform>();
    private Color mainColor = Color.white;
    private float startAlpha = 1f;
    
    public void Init(float lifetime, float widthMultiplier = 1f, float startAlpha = 1f)
    {
        this.startAlpha = startAlpha;
        SetColor(Color.white);
        StartCoroutine(FadeOut(lifetime));
        
        foreach (var lineRenderer in lineRenderers)
        {
            float startWidth = lineRenderer.startWidth;
            float endWidth = lineRenderer.endWidth;
            lineRenderer.startWidth = startWidth * widthMultiplier;
            lineRenderer.endWidth = endWidth * widthMultiplier;
        }
    }
    
    public void SetColor(Color color)
    {
        mainColor = color;
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.material.color = color;
        }
    }
    
    public void SetOrderInLayer(int order)
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.sortingOrder = order;
        }
    }
    

    private void Update()
    {
        if (points.Count == 0)
        {
            return;
        }

        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i].position;
        }
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.loop = true;
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(positions);
        }
    }
    
    public void AddPoint(Transform point)
    {
        points.Add(point);
    }
    
    private IEnumerator FadeOut(float lifetime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            float alpha = alphaCurve.Evaluate((Time.time - startTime) / lifetime) * startAlpha;

            Color col = mainColor;
            col.a = alpha;
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.material.color = col;
            }

            yield return null;
        }
        Destroy(gameObject);
    }
}
