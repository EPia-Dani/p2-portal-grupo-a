using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{

    public GameObject pressableButton;

    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Cube"))
        {
            Debug.Log("OOoh a cube");
            pressableButton.transform.position -= new Vector3(0, 0.1f, 0);

            // open door or something else

        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            pressableButton.transform.position += new Vector3(0, 0.1f, 0);
            StartCoroutine(DisableButtonTemporaly());

            // open door or something else
        }
    }

    IEnumerator DisableButtonTemporaly()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<BoxCollider>().enabled = true;
    }

}
