using System.Collections;
using UnityEngine;

public class PumpController : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform instantiatePlace;

    public GameObject pressableButton;

    public int maxInstantiateNumber = 2;
    public int currentInstantiateNumber = 0;

    private bool canPress = true;

    public void InstanceBox()
    {
        if (canPress)
        {
            StartCoroutine(PressButton());
            if (currentInstantiateNumber < maxInstantiateNumber)
            {
                Instantiate(boxPrefab, instantiatePlace.position, Quaternion.identity);
                currentInstantiateNumber += 1;
            }
        }
    }
    IEnumerator PressButton()
    {
        canPress = false;
        pressableButton.transform.position -= new Vector3(0, 0.1f, 0);
        yield return new WaitForSeconds(0.5f);
        pressableButton.transform.position += new Vector3(0, 0.1f, 0);
        StartCoroutine(DisableButtonTemporaly());
    }

    IEnumerator DisableButtonTemporaly()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<BoxCollider>().enabled = true;
        canPress = true;
    }


}