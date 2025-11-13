using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathController : MonoBehaviour
{
    public GameObject gameoverHUD;
    public GameObject player;
    public Checkpoint checkpoint;
    private FPSController playerController;
    private ShootPortal shootPortal;
    private Vector3 playerPos;
    private Quaternion playerRot;

    private void Start()
    {
        gameoverHUD.SetActive(false);
        playerRot = player.transform.rotation;
        playerPos = player.transform.position;
        playerController = player.GetComponent<FPSController>();
        shootPortal = playerController.GetComponent<ShootPortal>();
    }

    public void PlayerDead()
    {
        gameoverHUD.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null)
            playerController.enabled = false;
        if (shootPortal != null)
            shootPortal.enabled = false;
    }

    public void RespawnStartPosition()
    {
        gameoverHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        player.transform.position = checkpoint.GetStartPosition();
        /*player.transform.rotation = playerRot;
        player.transform.position = playerPos;*/
    }

    public void RespawnCheckpointPosition()
    {
        gameoverHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        player.transform.position = checkpoint.GetCheckpointPosition();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
