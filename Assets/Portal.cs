using Unity.Cinemachine;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private CinemachineCamera playerCamera;

    public Transform reflectionTransform;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPosition = reflectionTransform.InverseTransformPoint(playerCamera.transform.position);
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        Vector3 localDirection = reflectionTransform.InverseTransformDirection(playerCamera.transform.forward);
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);

        /*offsetNearPlane = */
    }
}
