using UnityEngine;

public class WinCheckpointCollider : MonoBehaviour
{
    private Win win;
    private bool isReached = false;

    private void Start()
    {
        win = GameObject.Find("WinManager").GetComponent<Win>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isReached)
        {
            SoundManager.Instance.PlaySFX("confeti");
            SoundManager.Instance.PlaySFX("oiia");
            isReached = true;
            win.playerWin();
        }
    }
}
