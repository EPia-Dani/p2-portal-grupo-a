using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 5f;
    public PlayerHUD playerHUD;
    public GameObject pauseHUD;

    private bool pause = false;

    private void Update()
    {
        HandleCast(false);
    }

    public void HandleCast(bool instBox)
    {
        Vector3 shootDirection = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = playerCamera.ViewportPointToRay(shootDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            PumpController pump = hit.collider.GetComponent<PumpController>();
            if (pump != null)
            {
                playerHUD.ShowInteractionText("Press E to drop box");
                if (instBox)
                {
                    pump.InstanceBox();
                    SoundManager.Instance.PlaySFX("cube_spawn");
                }    
                return;
            }
        }

        playerHUD.HideInteractionText();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleCast(true);
        }
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (pause)
        {
            Time.timeScale = 1f;
            pause = false;
            pauseHUD.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0f;
            pause = true;
            pauseHUD.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
            
    }
}
