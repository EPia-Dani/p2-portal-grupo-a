using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPortal : MonoBehaviour
{
    private Camera playerCamera;
    private float shotDelay = .15f;
    private bool isFiring = false;

    public Material singlePortalMaterial;
    public Material bluePortalMaterial;
    public Material orangePortalMaterial;

    public GameObject bluePortalPrefab;
    public GameObject orangePortalPrefab;
    
    private GameObject bluePortalGameObject;
    private GameObject orangePortalGameObject;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && !isFiring)
        {
            StartCoroutine(FireCoroutine(true));
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed && !isFiring)
        {
            StartCoroutine(FireCoroutine(false));
        }
    }

    private IEnumerator FireCoroutine(bool isBluePortal)
    {
        isFiring = true;
        Fire(isBluePortal);
        yield return new WaitForSeconds(shotDelay);
    }

    private void Fire(bool isBluePortal)
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj != null)
            {
                if (hitObj.layer == 7)
                {
                    Vector3 hitPoint = hit.point + hit.normal * 0.01f;
                    Quaternion rotacion = Quaternion.LookRotation(hit.normal);

                    if (isBluePortal)
                    {
                        if (bluePortalGameObject != null)
                        {
                            Destroy(bluePortalGameObject);
                        }

                        bluePortalGameObject = Instantiate(bluePortalPrefab, hitPoint, rotacion);
                        if (orangePortalGameObject != null)
                        {
                            Portal bluePortal = bluePortalGameObject.GetComponent<Portal>();
                            Portal orangePortal = orangePortalGameObject.GetComponent<Portal>();

                            bluePortal.mirrorPortal = orangePortal;
                            orangePortal.mirrorPortal = bluePortal;
                        }
                    }
                    else
                    {
                        if (orangePortalGameObject != null)
                        {
                            Destroy(orangePortalGameObject);
                        }

                        orangePortalGameObject = Instantiate(orangePortalPrefab, hitPoint, rotacion);
                        if (bluePortalGameObject != null)
                        {
                            Portal orangePortal = orangePortalGameObject.GetComponent<Portal>();
                            Portal bluePortal = bluePortalGameObject.GetComponent<Portal>();

                            orangePortal.mirrorPortal = bluePortal;
                            bluePortal.mirrorPortal = orangePortal;
                        }
                    }

                    UpdatePortalMaterial();
                }
            }
        }

        isFiring = false;
    }

    private void UpdatePortalMaterial()
    {
        bool doublePortal = (orangePortalGameObject != null && bluePortalGameObject != null);

        if (orangePortalGameObject != null)
        {
            MeshRenderer rend = orangePortalGameObject.GetComponentInChildren<MeshRenderer>();
            Material[] materials = rend.materials;
            materials[0] = doublePortal ? orangePortalMaterial : singlePortalMaterial;
            rend.materials = materials;
        }
        
        if (bluePortalGameObject != null)
        {
            MeshRenderer rend = bluePortalGameObject.GetComponentInChildren<MeshRenderer>();
            Material[] materials = rend.materials;
            materials[0] = doublePortal ? bluePortalMaterial : singlePortalMaterial;
            rend.materials = materials;
        }
    }

    private bool isValidPosition(GameObject portalPrefab)
    {
        bool illegalPos = false;
        Transform points = portalPrefab.transform.Find("ValidPosition");
        List<Transform> ValidPoints = points.GetComponentsInChildren<Transform>().ToList();

        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj != null)
            {
                if (hitObj.layer != 7)
                {
                    illegalPos = true;
                    return illegalPos;
                }

                Vector3 hitPoint = hit.point + hit.normal * 0.01f;
                Quaternion rotacion = Quaternion.LookRotation(hit.normal);
                Instantiate(points, hitPoint, rotacion);

                //TODO: refactorizar
                foreach (Transform t in ValidPoints)
                {
                    Ray ray2 = playerCamera.ViewportPointToRay(t.position);
                    RaycastHit hit2;
                    if (Physics.Raycast(ray2, out hit2))
                    {
                        GameObject hitObj2 = hit2.collider.gameObject;
                        if (hitObj2 != null)
                        {
                            if (hitObj2.layer != 7) illegalPos = true;
                        }
                    }
                }


            }
        }

        return illegalPos;
    }
}
