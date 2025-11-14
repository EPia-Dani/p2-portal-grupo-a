using UnityEngine;

public class WinCheckpointCollider : MonoBehaviour
{
    private WinController winController;
    private bool isReached = false;

    private void Start()
    {
        winController = GameObject.Find("WinManager").GetComponent<WinController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isReached)
        {
            isReached = true;
            winController.playerWin();
        }
    }
}
