using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Camera playerCamera;

    public Transform reflectionTransform;
    public Camera reflectionCamera;
    public Portal mirrorPortal;
    public float offsetNearPlane = 1.3f;

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 localPosition = reflectionTransform.InverseTransformPoint(playerCamera.transform.position);
        localPosition.z = -localPosition.z;
        localPosition.x = -localPosition.x;
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        Vector3 localDirection = reflectionTransform.InverseTransformDirection(playerCamera.transform.forward);
        localDirection.z = -localDirection.z;
        localDirection.x = -localDirection.x;
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);

        float distance = Vector3.Distance(mirrorPortal.reflectionCamera.transform.position, mirrorPortal.transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = distance + offsetNearPlane;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject player = GameObject.Find("Player");
            CharacterController characterController = player.GetComponent<CharacterController>();
            FPSController fpsController = player.GetComponent<FPSController>();

            Debug.Log("before player: " + player.transform.position);
            characterController.enabled = false;
            fpsController.enabled = false;

            Vector3 localPosition = reflectionTransform.InverseTransformPoint(player.transform.position);
            localPosition.x = -localPosition.x;
            Vector3 worldPosition = mirrorPortal.transform.TransformPoint(localPosition) + mirrorPortal.transform.forward * 0.1f;
            player.transform.position = worldPosition;

            //TODO: Girar el personaje cuando pasa por el portal
            /*Vector3 localDirection = reflectionTransform.InverseTransformDirection(playerCamera.transform.forward);
            localDirection.z = -localDirection.z;
            localDirection.x = -localDirection.x;*/
            /*Quaternion rotation = Quaternion.Inverse(mirrorPortal.reflectionTransform.rotation) * player.transform.rotation;
            player.transform.rotation = mirrorPortal.transform.rotation * rotation;*/

            characterController.enabled = true;
            fpsController.enabled = true;
            Debug.Log("after player: " + worldPosition);


            StartCoroutine(DisablePortalTemporaly());
        }
    }

    IEnumerator DisablePortalTemporaly()
    {
        mirrorPortal.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1f);
        mirrorPortal.GetComponent<Collider>().enabled = true;
    }
}
