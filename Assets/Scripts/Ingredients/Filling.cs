using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Filling : MonoBehaviour
{
    public FillingType type;

    public bool IsUsed { get; private set; }

    public void Use()
    {
        if (IsUsed) return;
        IsUsed = true;
        gameObject.SetActive(false);
    }
}
