using UnityEngine;

public class CakeFillingArea : MonoBehaviour
{
    [Header("Colocacion")]
    public float centerRadius = 0.1f;

    [Header("Esparcido")]
    public float spreadSpeed = 0.4f;
    public float spreadVelocityThreshold = 0.5f;

    [Range(0f, 1f)]
    public float spreadProgress;

    public FillingType? AppliedFilling { get; private set; }
    public bool IsFillingPlaced { get; private set; }
    public bool IsSpreadComplete => spreadProgress >= 1f;

    private GameObject currentSpatula;
    private Vector3 lastSpatulaPosition;

    public System.Action<FillingType> OnFillingApplied;
    public System.Action OnSpreadComplete;

    void Update()
    {
        if (!IsFillingPlaced || IsSpreadComplete || currentSpatula == null) return;

        float velocity = (currentSpatula.transform.position - lastSpatulaPosition).magnitude / Time.deltaTime;

        if (velocity >= spreadVelocityThreshold)
        {
            spreadProgress = Mathf.Clamp01(spreadProgress + spreadSpeed * Time.deltaTime);

            if (IsSpreadComplete)
                OnSpreadComplete?.Invoke();
        }

        lastSpatulaPosition = currentSpatula.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsFillingPlaced)
        {
            Filling filling = other.GetComponent<Filling>();
            if (filling != null && !filling.IsUsed && IsNearCenter(other.transform.position))
            {
                AppliedFilling = filling.type;
                IsFillingPlaced = true;
                filling.Use();
                OnFillingApplied?.Invoke(filling.type);
                return;
            }
        }

        if (other.CompareTag("Spatula"))
        {
            currentSpatula = other.gameObject;
            lastSpatulaPosition = currentSpatula.transform.position;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spatula"))
            currentSpatula = null;
    }

    bool IsNearCenter(Vector3 position)
    {
        Vector3 flat = new Vector3(position.x, transform.position.y, position.z);
        return Vector3.Distance(flat, transform.position) <= centerRadius;
    }
}
