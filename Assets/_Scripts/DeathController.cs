using System.Collections;
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
    private bool isDead = false;

    private void Start()
    {
        gameoverHUD.SetActive(false);
        playerController = player.GetComponent<FPSController>();
        shootPortal = playerController.GetComponent<ShootPortal>();
    }

    public void PlayerDead()
    {
        if (isDead) return;

        isDead = true;
        gameoverHUD.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null)
            playerController.enabled = false;
        if (shootPortal != null)
            shootPortal.enabled = false;
    }

    public void RespawnStartPosition()
    {
        player.transform.position = checkpoint.GetStartPosition();
        gameoverHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        StartCoroutine(InmunePeriode());
    }

    public void RespawnCheckpointPosition()
    {
        player.transform.position = checkpoint.GetCheckpointPosition();
        gameoverHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
            playerController.enabled = true;
        if (shootPortal != null)
            shootPortal.enabled = true;

        StartCoroutine(InmunePeriode());
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }

    IEnumerator InmunePeriode()
    {
        yield return new WaitForSeconds(0.5f);
        isDead = false;
    }
}
