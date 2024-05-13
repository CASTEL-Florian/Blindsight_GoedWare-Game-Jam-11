using UnityEngine;


// Since all the objects in the game are invisible, I created this script to draw boundaries around them in the editor.
public class DrawBoundaries : MonoBehaviour
{
    public enum BoundaryType
    {
        Box,
        Circle
    }
    [SerializeField] private Color color = Color.red;
    [SerializeField] private BoundaryType boundaryType = BoundaryType.Box;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (boundaryType == BoundaryType.Box)
        {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
        else if (boundaryType == BoundaryType.Circle)
        {
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }
    }
}