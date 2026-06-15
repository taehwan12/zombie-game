using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour {
    private LineRenderer lineRenderer;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Setup(Vector3 start, Vector3 end, float width, Color startColor, Color endColor) {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct() {
        yield return new WaitForSeconds(0.03f);
        Destroy(gameObject);
    }
}