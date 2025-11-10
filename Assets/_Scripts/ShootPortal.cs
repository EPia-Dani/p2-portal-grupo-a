using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class ShootPortal : MonoBehaviour
{
    private Camera playerCamera;
    private float shotDelay = .15f;
    private bool isFiring = false;
    private float cubeDistance = 2f;
    private bool haveCube = false;
    private GameObject cube;
    private float throwCubeSpeed = 8f;

    public Transform attachPosition;

    public Material singlePortalMaterial;
    public Material bluePortalMaterial;
    public Material orangePortalMaterial;

    public GameObject bluePortalPrefab;
    public GameObject orangePortalPrefab;

    public GameObject bluePortalSprite;
    public GameObject orangePortalSprite;

    public CrosshairController crosshairController;
    
    private GameObject bluePortalGameObject;
    private GameObject orangePortalGameObject;
    private bool isHoldingFire = false;
    private bool isHoldingRight = false;

    private Vector2 scrollValue;

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
        UpdateCubeMovement();
        
    }

    public void OnScrollValueChanged(InputAction.CallbackContext context)
    {
        scrollValue = context.ReadValue<Vector2>();
        StartCoroutine(ResetScrollValueCoroutine());
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
            StartCoroutine(FireCoroutine());
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
            StartCoroutine(RightCoroutine());
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
            if (scrollValue.y > 0) 
            {
                Debug.Log("enlarge");
                portalSprite.transform.localScale += Vector3.one * 1.02f;
            }
            else if (scrollValue.y < 0)
            {
                Debug.Log("reduce");
                portalSprite.transform.localScale -= Vector3.one * 1.02f;
            }
        }
        else
        {
            portalSprite.SetActive(false);
        }
    }

    private IEnumerator ResetScrollValueCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        scrollValue = Vector2.zero;
    }

    private IEnumerator FireCoroutine()
    {
        isFiring = true;
        Fire();
        yield return new WaitForSeconds(shotDelay);
    }
    private IEnumerator RightCoroutine()
    {
        isFiring = true;
        Right();
        yield return new WaitForSeconds(shotDelay);
    }

    private void Fire()
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        FireCube();
        CatchCube();

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.layer == 7)
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.01f;
                Quaternion rotacion = Quaternion.LookRotation(hit.normal);

                if (isValidPosition())
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

                UpdatePortalMaterial();
                UpdatePortalCrosshair();
            }
        }

        isFiring = false;
    }
    
    private void Right()
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        ReleaseCube();

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.layer == 7)
            {
                Vector3 hitPoint = hit.point + hit.normal * 0.01f;
                Quaternion rotacion = Quaternion.LookRotation(hit.normal);

                if (isValidPosition())
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
                UpdatePortalCrosshair();
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

    private void UpdatePortalCrosshair()
    {
        if (orangePortalGameObject == null && bluePortalGameObject == null)
        {
            crosshairController.SetNoPortal();
        }
        else if (orangePortalGameObject != null && bluePortalGameObject == null)
        {
            crosshairController.SetOrangePortal();
        }
        else if (orangePortalGameObject == null && bluePortalGameObject != null)
        {
            crosshairController.SetBluePortal();
        }
        else if (orangePortalGameObject != null && bluePortalGameObject != null)
        {
            crosshairController.SetBothPortal();
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

    private void CatchCube()
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, cubeDistance))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.tag == "Cube")
            {
                if (!haveCube)
                {
                    cube = hitObj;
                    haveCube = true;
                    Rigidbody cubeRidigbody = hitObj.GetComponent<Rigidbody>();
                    cubeRidigbody.useGravity = false;

                    hitObj.transform.SetParent(attachPosition);
                    hitObj.transform.localPosition = Vector3.zero;
                    hitObj.transform.localRotation = Quaternion.identity;
                }
            }
            else if (hitObj.tag == "Turret")
            {
                if (!haveCube)
                {
                    cube = hitObj;
                    haveCube = true;
                    Rigidbody cubeRidigbody = hitObj.GetComponent<Rigidbody>();
                    cubeRidigbody.useGravity = false;

                    hitObj.transform.SetParent(attachPosition);
                    hitObj.transform.localPosition = Vector3.zero;
                    hitObj.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }

    private void FireCube()
    {
        if (haveCube)
        {
            StartCoroutine(FireCubeCoroutine());
        }
    }

    IEnumerator FireCubeCoroutine() {

        yield return new WaitForSeconds(0.05f);

        Rigidbody cubeRidigbody = cube.GetComponent<Rigidbody>();
        cubeRidigbody.useGravity = true;
        cube.transform.SetParent(null);

        Vector3 direction = playerCamera.transform.forward;
        cubeRidigbody.linearVelocity = direction * throwCubeSpeed;

        haveCube = false;
        cube = null;
    }

private void ReleaseCube()
    {
        if (haveCube)
        {
            Rigidbody cubeRidigbody = cube.GetComponent<Rigidbody>();
            cubeRidigbody.useGravity = true;
            cube.transform.SetParent(null);
            haveCube = false;
            cube = null;
        }
    }


    private void UpdateCubeMovement()
    {
        if (haveCube && cube != null)
        {
            Vector3 targetPos = attachPosition.position;
            Vector3 direction = (targetPos - cube.transform.position);
            float distance = direction.magnitude;

            Rigidbody cubeRidigbody = cube.GetComponent<Rigidbody>();
            cubeRidigbody.linearVelocity = direction * distance;
        }
    }
}
