using UnityEngine;

public class DeathController : MonoBehaviour
{
    public GameObject gameoverHUD;
    public GameObject player;
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

        if (playerController != null)
            playerController.enabled = false;
        if (shootPortal != null)
            shootPortal.enabled = false;
    }

    public void playerRevive()
    {
        gameoverHUD.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        player.transform.rotation = playerRot;
        player.transform.position = playerPos;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
