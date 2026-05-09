using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Ingredient : MonoBehaviour
{
    public IngredientType type;
    public float amount = 1f;

    public bool IsUsed { get; private set; }

    public void Use()
    {
        if (IsUsed) return;
        IsUsed = true;
        gameObject.SetActive(false);
    }
}
