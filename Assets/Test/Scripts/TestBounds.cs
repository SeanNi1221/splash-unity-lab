using UnityEngine;

public class TestBounds : MonoBehaviour
{
    // Draws a wireframe box around the selected object,
    // indicating local space bounding volume.
    public void OnDrawGizmosSelected()
    {
        var r = GetComponent<Renderer>();
        if (r == null)
            return;
        var localBounds = r.localBounds;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(localBounds.center, localBounds.extents * 2);

        var bounds = r.bounds;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
    }
}