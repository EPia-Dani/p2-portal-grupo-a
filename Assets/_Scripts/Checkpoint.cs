using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject player;
    public PlayerHUD playerHUD;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 checkpointPosition = Vector3.zero;
    private Quaternion checkpointRotation;

    void Start()
    {
        startPosition = player.transform.position;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
        playerHUD.ShowInteractionTextWithDuration("New checkpoint reached!");
    }

    public Vector3 GetCheckpointPosition()
    {
        if (checkpointPosition == Vector3.zero) { 
            return startPosition;
        }
        return checkpointPosition;
    }
}
