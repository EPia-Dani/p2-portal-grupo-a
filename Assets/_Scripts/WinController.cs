using UnityEngine;

public class WinController : MonoBehaviour
{
    public GameObject meme;
    public GameObject meme2;
    public GameObject meme3;
    public GameObject meme4;
    public GameObject meme5;
    public ParticleSystem particleSystem;

    private void Start()
    {
        meme.SetActive(false);
        meme2.SetActive(false);
        meme3.SetActive(false);
        meme4.SetActive(false);
        meme5.SetActive(false);
        particleSystem.Stop();
    }

    public void playerWin()
    {
        SoundManager.Instance.StopMusic();
        meme.SetActive(true);
        meme2.SetActive(true);
        meme3.SetActive(true);
        meme4.SetActive(true);
        meme5.SetActive(true);
        particleSystem.Play();
    }
}
