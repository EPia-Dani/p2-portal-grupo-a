using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI interactionText;

    public void ShowInteractionText(string message)
    {
        interactionText.text = message;
        interactionText.enabled = true;
    }
    
    public void ShowInteractionTextWithDuration(string message)
    {
        interactionText.text = message;
        interactionText.enabled = true;
        StartCoroutine(CountdownShowTextTime());
    }

    public void HideInteractionText()
    {
        interactionText.enabled = false;
    }

    IEnumerator CountdownShowTextTime()
    {
        yield return new WaitForSeconds(2f);
        HideInteractionText();
    }
}
