using UnityEngine;

public class PumpController : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform instantiatePlace;
    public int maxInstantiateNumber = 2;
    public int currentInstantiateNumber = 0;

    public void InstanceBox()
    {
        if (currentInstantiateNumber < maxInstantiateNumber)
        {
            Instantiate(boxPrefab, instantiatePlace.position, Quaternion.identity);
            currentInstantiateNumber += 1;
        }
    }

}
