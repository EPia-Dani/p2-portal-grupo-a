using Unity.Cinemachine;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Camera playerCamera;

    public Transform reflectionTransform;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 localPosition = reflectionTransform.InverseTransformPoint(playerCamera.transform.position);
        localPosition.z = -localPosition.z;
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        Vector3 localDirection = reflectionTransform.InverseTransformDirection(playerCamera.transform.forward);
        localDirection.z = -localDirection.z;
        localDirection.x = -localDirection.x;
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);

        float distance = Vector3.Distance(mirrorPortal.reflectionCamera.transform.position, mirrorPortal.transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = distance + offsetNearPlane;
    }
}
