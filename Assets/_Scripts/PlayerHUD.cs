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

    public void HideInteractionText()
    {
        interactionText.enabled = false;
    }
}
