using Unity.VisualScripting;
using UnityEngine;

public class CheckpointCollision : MonoBehaviour 
{
    private Checkpoint checkpoint;
    private bool isReached = false;

    private void Start()
    {
        checkpoint = GameObject.Find("CheckpointManager").GetComponent<Checkpoint>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isReached)
        {
            SoundManager.Instance.PlaySFX("checkpoint");
            isReached = true;
            checkpoint.SetCheckpoint(transform.position);
        }
    }
}
