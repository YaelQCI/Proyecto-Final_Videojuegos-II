using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CreamDispenser : MonoBehaviour
{
    public CreamType type;
    public InputActionReference triggerAction;

    public bool IsActive => triggerAction != null && triggerAction.action.IsPressed();
}
