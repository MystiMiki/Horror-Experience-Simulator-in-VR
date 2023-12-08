using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClipboardPickupTracker : MonoBehaviour
{
    public static bool isInHand = false; // Track whether the player has the clipboard in hand

    // Called when the player picks up the clipboard
    public void OnClipboardPickedUp()
    {
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null && !string.IsNullOrEmpty(textComponent.text))
        {
            // Log the event when the clipboard is picked up with text
            isInHand = true;
        }        
    }

    // Called when the player drops the clipboard
    public void OnClipboardDropped()
    {
        TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null && !string.IsNullOrEmpty(textComponent.text))
        {
            // Log the event when the clipboard is picked up with text
            isInHand = false;
        }        
    }
}
