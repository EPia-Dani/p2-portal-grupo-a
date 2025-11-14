using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathController : MonoBehaviour
{
    public GameObject gameoverHUD;
    public GameObject player;
    public Checkpoint checkpoint;
    private CharacterController characterController;
    private FPSController fpsController;
    private ShootPortal shootPortal;
    private PauseController pauseController;
    private bool isDead = false;

    private void Start()
    {
        gameoverHUD.SetActive(false);
        fpsController = player.GetComponent<FPSController>();
        shootPortal = fpsController.GetComponent<ShootPortal>();
        characterController = player.GetComponent<CharacterController>();
        pauseController = GameObject.Find("PauseManager").GetComponent<PauseController>();
    }

    public void PlayerDead()
    {
        if (isDead) return;

        isDead = true;
        gameoverHUD.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (fpsController != null)
            fpsController.enabled = false;
        if (shootPortal != null)
            shootPortal.enabled = false;
    }

    public void RespawnStartPosition()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void RespawnCheckpointPosition()
    {
        characterController.enabled = false;
        fpsController.enabled = false;

        player.transform.position = checkpoint.GetCheckpointPosition();
        Debug.Log("GetCheckpointPosition: " + checkpoint.GetCheckpointPosition());

        gameoverHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (fpsController != null)
            fpsController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        characterController.enabled = true;
        fpsController.enabled = true;
        StartCoroutine(InmunePeriode());
    }

    public void ExitGame()
    {
        pauseController.Resume();
        Cursor.lockState = CursorLockMode.None;
        SoundManager.Instance.StopLoop();
        SceneManager.LoadScene("MenuScene");
    }

    IEnumerator InmunePeriode()
    {
        yield return new WaitForSeconds(0.5f);
        isDead = false;
    }
}
