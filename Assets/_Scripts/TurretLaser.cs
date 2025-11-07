using UnityEngine;

public class TurretLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxDistance = 90f;
    public float angleLaser = 45f;
    public bool tempo;

    private void Start()
    {
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        Vector3 endPos;
        RaycastHit hit;
        if (Physics.Raycast(new(transform.position, transform.forward), out hit, maxDistance))
        {
            endPos = hit.point;
            tempo = true;
        }
        else
        {
            endPos = transform.position + transform.forward * maxDistance;
            tempo=false;
        }

        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, endPos);
    }
}
