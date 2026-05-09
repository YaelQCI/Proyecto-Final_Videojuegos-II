using UnityEngine;

public class CakeDecorationArea : MonoBehaviour
{
    [Header("Linea Guia")]
    public Transform[] guidePath;
    public float coverRadius = 0.05f;
    [Range(0f, 100f)]
    public float precisionThreshold = 70f;

    [Header("Toppings")]
    public int requiredToppings = 3;

    public float Precision { get; private set; }
    public int ToppingsPlaced { get; private set; }
    public CreamType? AppliedCream { get; private set; }

    public bool IsDecorationComplete =>
        Precision >= precisionThreshold && ToppingsPlaced >= requiredToppings;

    private bool[] coveredPoints;

    public System.Action<float> OnPrecisionUpdated;
    public System.Action<ToppingType> OnToppingPlaced;
    public System.Action OnDecorationComplete;

    void Awake()
    {
        coveredPoints = new bool[guidePath.Length];
    }

    void OnTriggerStay(Collider other)
    {
        CreamDispenser dispenser = other.GetComponent<CreamDispenser>();
        if (dispenser == null || !dispenser.IsActive) return;

        AppliedCream = dispenser.type;
        bool updated = false;

        for (int i = 0; i < guidePath.Length; i++)
        {
            if (coveredPoints[i]) continue;

            // Compara solo en el plano horizontal (ignora altura)
            Vector3 dispenserFlat = new Vector3(other.transform.position.x, guidePath[i].position.y, other.transform.position.z);
            if (Vector3.Distance(dispenserFlat, guidePath[i].position) <= coverRadius)
            {
                coveredPoints[i] = true;
                updated = true;
            }
        }

        if (updated)
            UpdatePrecision();
    }

    void OnTriggerEnter(Collider other)
    {
        Topping topping = other.GetComponent<Topping>();
        if (topping == null || topping.IsPlaced) return;

        topping.Place();
        ToppingsPlaced++;
        OnToppingPlaced?.Invoke(topping.type);

        if (IsDecorationComplete)
            OnDecorationComplete?.Invoke();
    }

    void UpdatePrecision()
    {
        if (guidePath.Length == 0) return;

        int covered = 0;
        foreach (bool point in coveredPoints)
            if (point) covered++;

        Precision = (float)covered / guidePath.Length * 100f;
        OnPrecisionUpdated?.Invoke(Precision);

        if (IsDecorationComplete)
            OnDecorationComplete?.Invoke();
    }
}
