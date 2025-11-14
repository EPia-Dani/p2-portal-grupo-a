using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    private bool pause = false;
    public GameObject pauseHUD;

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (!pause)
        {
            Time.timeScale = 0f;
            pause = true;
            pauseHUD.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            SoundManager.Instance.PauseLoop();
        }
    }

    public void Resume()
    {
        if (pause)
        {
            Time.timeScale = 1f;
            pause = false;
            pauseHUD.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            SoundManager.Instance.ResumeLoop();
        }
    }

    public void ExitGame()
    {
        Resume();
        Cursor.lockState = CursorLockMode.None;
        SoundManager.Instance.StopLoop();
        SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("MenuScene");
    }
}
