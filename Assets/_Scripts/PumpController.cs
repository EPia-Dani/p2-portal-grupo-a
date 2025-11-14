using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpController : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform instantiatePlace;

    public GameObject pressableButton;

    public int maxInstantiateNumber = 2;
    public int currentInstantiateNumber = 0;

    private bool canPress = true;

    private List<GameObject> spawnedBoxes = new List<GameObject>();

    public void InstanceBox()
    {
        if (!canPress) return;

        StartCoroutine(PressButton());

        // Si ya hay 2 cubos, eliminar el primero
        if (spawnedBoxes.Count >= maxInstantiateNumber)
        {
            Destroy(spawnedBoxes[0]);
            spawnedBoxes.RemoveAt(0);
        }

        // Crear un nuevo cubo
        GameObject newBox = Instantiate(boxPrefab, instantiatePlace.position, Quaternion.identity);

        // Guardarlo en la lista
        spawnedBoxes.Add(newBox);
    }
    IEnumerator PressButton()
    {
        SoundManager.Instance.PlaySFX("button_press");

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