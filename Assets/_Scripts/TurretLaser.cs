using UnityEngine;

public class TurretLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxDistance = 90f;
    public float angleLaser = 45f;
    public bool activate = true;

    private void Start()
    {
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.useWorldSpace = true;
        SoundManager.Instance.PlaySFX("turret_laser_start");
        SoundManager.Instance.PlaySFX("turret_laser_loop");
    }

    private void Update()
    {
        if (activate)
        {
            TurretLaset();
        }
    }

    private void TurretLaset()
    {
        Vector3 endPos;
        RaycastHit hit;
        if (Physics.Raycast(new(transform.position, transform.forward), out hit, maxDistance))
        {
            endPos = hit.point;

            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.tag == "Player")
            {
                DeathController deathController = hitObj.GetComponent<DeathController>();
            }
            else if (hitObj.tag == "Turret")
            {
                TurretLaser turretLaser = hitObj.GetComponent<TurretLaser>();
                SoundManager.Instance.PlaySFX("turret_die");
                turretLaser.TurretDeactivate();
            }
        }
        else
        {
            endPos = transform.position + transform.forward * maxDistance;
        }

        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, endPos);
    }

    public void TurretDeactivate()
    {
        if (activate)
        {
            lineRenderer.enabled = false;
            activate = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cube" || other.tag == "Turret")
        {
            SoundManager.Instance.PlaySFX("turret_die");
            TurretDeactivate();
        }
    }

}
