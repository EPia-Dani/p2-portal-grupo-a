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

            characterController.enabled = false;
            fpsController.enabled = false;

            Vector3 localPosition = reflectionTransform.InverseTransformPoint(player.transform.position);
            localPosition.x = -localPosition.x;
            Vector3 worldPosition = mirrorPortal.transform.TransformPoint(localPosition) + mirrorPortal.transform.forward * 0.1f;
            player.transform.position = worldPosition;

            // Giro de dirección:
            Vector3 localDirection =  reflectionTransform.InverseTransformDirection(playerCamera.transform.forward);
            localDirection = new Vector3(-localDirection.x, localDirection.y, -localDirection.z);

            Vector3 newDirection = mirrorPortal.transform.TransformDirection(localDirection);
            Quaternion newRotation = Quaternion.LookRotation(newDirection, mirrorPortal.transform.up);

            player.transform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);

            FPSController fps = player.GetComponent<FPSController>();
            fps.SetYaw(newRotation.eulerAngles.y);

            float pitch = newRotation.eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            fps.SetPitch(Mathf.Clamp(pitch, fps.mMinPitch, fps.mMaxPitch));
            fps.ApplyRotationImmediate();

            characterController.enabled = true;
            fpsController.enabled = true;

            StartCoroutine(DisablePortalTemporaly());
        }
        else if (other.tag == "Cube")
        {
            Debug.Log("found a cube!");
            Transform t = other.gameObject.transform;
            Vector3 localPosition = reflectionTransform.InverseTransformPoint(t.position);
            localPosition.x = -localPosition.x;
            Vector3 worldPosition = mirrorPortal.transform.TransformPoint(localPosition) + mirrorPortal.transform.forward * 0.1f;
            other.gameObject.transform.position = worldPosition;

            if (mirrorPortal.transform.localScale.x != 1)
            {
                Vector3 newScale = other.gameObject.transform.localScale * mirrorPortal.transform.localScale.x;
                other.gameObject.transform.localScale = Vector3.one * Mathf.Clamp(newScale.x, 0.5f, 1.5f);
            }
            else other.transform.localScale = Vector3.one;

            
            //other.gameObject.transform.localScale += t.localScale * mirrorPortal.transform.localScale.x;

            Rigidbody cubeRigid = other.gameObject.GetComponent<Rigidbody>();

            // esto ajusta la rotación y posición del cubo sobre sí mismo, pero no la dirección en la que se mueve.
            // No sé ni si esto es necesario teniendo en cuenta que las 6 caras son idénticas.
                Vector3 moveDirection = playerCamera.transform.forward;
                Vector3 localDirection = reflectionTransform.InverseTransformDirection(moveDirection);

                localDirection = new Vector3(-localDirection.x, localDirection.y, -localDirection.z);
                Vector3 newDirection = mirrorPortal.transform.TransformDirection(localDirection);

                Quaternion newRotation = Quaternion.LookRotation(newDirection, mirrorPortal.transform.up);

                cubeRigid.linearVelocity = newDirection * cubeRigid.linearVelocity.magnitude;
                other.gameObject.transform.rotation = newRotation;

            
            //todo: calcular dirección en la que se mueve el cubo, para cambiársela cuando se teletransporta.

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
