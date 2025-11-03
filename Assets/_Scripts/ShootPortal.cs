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

    public GameObject bluePortalSprite;
    public GameObject orangePortalSprite;
    
    private GameObject bluePortalGameObject;
    private GameObject orangePortalGameObject;
    private bool isHoldingFire = false;
    private bool isHoldingRight = false;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (isHoldingFire)
        {
            DrawPortalSprite(bluePortalSprite);
        }
        if (isHoldingRight)
        {
            DrawPortalSprite(orangePortalSprite);
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingFire = true;
        }
        if (context.canceled && !isFiring)
        {
            isHoldingFire = false;
            bluePortalSprite.SetActive(false);
            StartCoroutine(FireCoroutine(true));
        }
    }
    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingRight = true;
        }
        if (context.canceled && !isFiring)
        {
            isHoldingRight = false;
            orangePortalSprite.SetActive(false);
            StartCoroutine(FireCoroutine(false));
        }
    }

    private void DrawPortalSprite(GameObject portalSprite)
    {
        if (isValidPosition())
        {
            Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
            Ray ray = playerCamera.ViewportPointToRay(shootDirection);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                portalSprite.SetActive(true);
                portalSprite.transform.position = hit.point + hit.normal * 0.01f;
                portalSprite.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
        else
        {
            portalSprite.SetActive(false);
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
            if (hitObj.layer == 7)
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.01f;
                Quaternion rotacion = Quaternion.LookRotation(hit.normal);

                if (isBluePortal && isValidPosition())
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
                else if (!isBluePortal && isValidPosition())
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

    private bool isValidPosition()
    {
        bool illegalPos = true;
        Transform points = GameObject.Find("ValidPosition").transform;
        Transform[] ValidPoints = new Transform[points.childCount];
        for (int i = 0; i < points.childCount; i++)
        {
            ValidPoints[i] = points.GetChild(i);
        }

        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.layer != 7)
            {
                illegalPos = false;
                return illegalPos;
            }

            points.position = hit.point + hit.normal * 0.01f;
            points.rotation = Quaternion.LookRotation(hit.normal);

            //TODO: refactorizar
            foreach (Transform t in ValidPoints)
            {
                Vector3 viewportPos = playerCamera.WorldToViewportPoint(t.position);
                Ray ray2 = playerCamera.ViewportPointToRay(viewportPos);
                //Debug.DrawLine(playerCamera.transform.position, t.position, Color.red, 100f);
                RaycastHit hit2;
                if (Physics.Raycast(ray2, out hit2))
                {
                    GameObject hitObj2 = hit2.collider.gameObject;
                    if (hitObj2 != null)
                    {
                        if (hitObj2.layer != 7) illegalPos = false;
                    }
                }
                else
                {
                    illegalPos = false;
                }
            }
        }
        return illegalPos;
    }
}
