using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CakeBox : MonoBehaviour
{
    public bool HasCake { get; private set; }
    public CakeDecorationArea CakeInside { get; private set; }

    public System.Action OnCakePlaced;

    void OnTriggerEnter(Collider other)
    {
        if (HasCake) return;

        CakeDecorationArea cake = other.GetComponent<CakeDecorationArea>();
        if (cake == null || !cake.IsDecorationComplete) return;

        CakeInside = cake;
        HasCake = true;

        // Mete el pastel dentro de la caja
        cake.transform.SetParent(transform);
        cake.transform.localPosition = Vector3.zero;

        OnCakePlaced?.Invoke();
    }
}
