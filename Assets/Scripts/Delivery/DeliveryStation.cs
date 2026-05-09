using UnityEngine;

public class DeliveryStation : MonoBehaviour
{
    public System.Action<CakeBox> OnBoxDelivered;

    void OnTriggerEnter(Collider other)
    {
        CakeBox box = other.GetComponent<CakeBox>();
        if (box == null || !box.HasCake) return;

        OnBoxDelivered?.Invoke(box);
        box.gameObject.SetActive(false);
    }
}
