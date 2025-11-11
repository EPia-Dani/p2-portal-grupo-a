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
    private Vector3 spriteOriginalScale;

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
    private float bluePortalScaleValue = 1;
    private float orangePortalScaleValue = 1;
    private Vector3 maxSize;
    private Vector3 minSize;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        spriteOriginalScale = bluePortalSprite.transform.localScale;
        maxSize = new Vector3(1.5f, 1.5f, 1.5f);
        minSize = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void Update()
    {
        if (isHoldingFire)
        {
            bluePortalScaleValue = DrawPortalSprite(bluePortalSprite, bluePortalScaleValue);
        }
        if (isHoldingRight)
        {
            orangePortalScaleValue = DrawPortalSprite(orangePortalSprite, orangePortalScaleValue);
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
            bluePortalScaleValue = 1;
            bluePortalSprite.transform.localScale = spriteOriginalScale;
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
            orangePortalScaleValue = 1;
            orangePortalSprite.transform.localScale = spriteOriginalScale;
            isHoldingRight = true;
        }
        if (context.canceled && !isFiring)
        {
            isHoldingRight = false;
            orangePortalSprite.SetActive(false);
            StartCoroutine(RightCoroutine());
        }
    }

    private float DrawPortalSprite(GameObject portalSprite, float portalScale)
    {
        if (isValidPosition())
        {
            Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
            Ray ray = playerCamera.ViewportPointToRay(shootDirection);
            RaycastHit hit;
            Vector3 newTransform = portalSprite.transform.localScale;
            
            if (Physics.Raycast(ray, out hit))
            {
                portalSprite.SetActive(true);
                Transform t = portalSprite.transform;
                t.position = hit.point + hit.normal * 0.01f;
                t.rotation = Quaternion.LookRotation(hit.normal);

                if (scrollValue.y != 0)
                {
                    newTransform += spriteOriginalScale * 0.3f * Mathf.Sign(scrollValue.y);
                    portalScale += portalScale * 0.3f * Mathf.Sign(scrollValue.y);
                    portalScale = Mathf.Clamp(portalScale, 0.5f, 1.5f);

                    t.localScale = spriteOriginalScale * portalScale;

                }
            }

            float factor = newTransform.x / spriteOriginalScale.x;
            factor = Mathf.Clamp(factor, 0.5f, 1.5f);
            portalSprite.transform.localScale = spriteOriginalScale * factor;

        }
        else
        {
            portalSprite.SetActive(false);
        }
        return portalScale;
    }

    private IEnumerator ResetScrollValueCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        scrollValue = Vector2.zero;
    }

    private IEnumerator FireCoroutine()
    {
        isFiring = true;
        SoundManager.Instance.PlaySFX("shoot_portal");
        Fire();
        yield return new WaitForSeconds(shotDelay);
    }
    private IEnumerator RightCoroutine()
    {
        isFiring = true;
        SoundManager.Instance.PlaySFX("shoot_portal");
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
                    float newScale = Mathf.Clamp(bluePortalScaleValue, 0.5f, 1.5f);
                    bluePortalGameObject.transform.localScale = Vector3.one * newScale;

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
                    float newScale = Mathf.Clamp(orangePortalScaleValue, 0.5f, 1.5f);
                    orangePortalGameObject.transform.localScale = Vector3.one * newScale;

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
                    SoundManager.Instance.PlaySFX("cube_pickup");
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
                    SoundManager.Instance.PlaySFX("cube_pickup");
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
        SoundManager.Instance.PlaySFX("cube_throw");

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
            SoundManager.Instance.PlaySFX("cube_release");

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
