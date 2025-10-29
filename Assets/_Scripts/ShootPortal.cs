using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPortal : MonoBehaviour
{
    private Camera playerCamera;
    private float shotDelay = .15f;
    private bool isFiring = false;
    private bool canFire = true;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && !isFiring)
        {
            StartCoroutine(FireCoroutine());
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed && !isFiring)
        {
            StartCoroutine(FireCoroutine());
        }
    }

    private IEnumerator FireCoroutine()
    {
        isFiring = true;

        while (isFiring && canFire)
        {
            Fire();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private void Fire()
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
        }
    }

}
