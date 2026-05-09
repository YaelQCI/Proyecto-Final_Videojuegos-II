using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Topping : MonoBehaviour
{
    public ToppingType type;

    public bool IsPlaced { get; private set; }

    public void Place()
    {
        if (IsPlaced) return;
        IsPlaced = true;
    }
}
