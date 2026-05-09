using UnityEngine;

public class Oven : MonoBehaviour
{
    public enum OvenState { Empty, Baking, NeedsFlipping, Baked, Burned }

    [Header("Tiempos")]
    public float bakingDuration = 30f;
    public float burnGracePeriod = 10f;

    [Header("Volteo")]
    [Range(0f, 1f)]
    public float flipThreshold = 0.5f;

    [Range(0f, 1f)]
    public float bakingProgress;
    public OvenState State { get; private set; } = OvenState.Empty;

    private MixingBowl cakeInside;
    private bool hasBeenFlipped;
    private float burnTimer;
    private Vector3 cakeUpAtEntry;

    public System.Action<OvenState> OnStateChanged;
    public System.Action OnFlipRequired;
    public System.Action OnCakeBaked;
    public System.Action OnCakeBurned;

    void Update()
    {
        switch (State)
        {
            case OvenState.Baking:      UpdateBaking();    break;
            case OvenState.NeedsFlipping: CheckFlip();    break;
            case OvenState.Baked:       UpdateBurnTimer(); break;
        }
    }

    void UpdateBaking()
    {
        bakingProgress = Mathf.Clamp01(bakingProgress + Time.deltaTime / bakingDuration);

        if (!hasBeenFlipped && bakingProgress >= flipThreshold)
        {
            SetState(OvenState.NeedsFlipping);
            return;
        }

        if (bakingProgress >= 1f)
            SetState(OvenState.Baked);
    }

    void CheckFlip()
    {
        if (cakeInside == null) return;

        // Detecta volteo cuando el objeto rota ~180 grados
        if (Vector3.Dot(cakeInside.transform.up, cakeUpAtEntry) < -0.5f)
        {
            hasBeenFlipped = true;
            cakeUpAtEntry = cakeInside.transform.up;
            SetState(OvenState.Baking);
        }
    }

    void UpdateBurnTimer()
    {
        burnTimer += Time.deltaTime;
        if (burnTimer >= burnGracePeriod)
            SetState(OvenState.Burned);
    }

    void OnTriggerEnter(Collider other)
    {
        if (State != OvenState.Empty) return;

        MixingBowl bowl = other.GetComponent<MixingBowl>();
        if (bowl != null && bowl.IsMixingComplete)
        {
            cakeInside = bowl;
            cakeUpAtEntry = bowl.transform.up;
            hasBeenFlipped = false;
            bakingProgress = 0f;
            burnTimer = 0f;
            SetState(OvenState.Baking);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (cakeInside != null && other.gameObject == cakeInside.gameObject)
        {
            cakeInside = null;
            SetState(OvenState.Empty);
        }
    }

    void SetState(OvenState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(State);

        switch (newState)
        {
            case OvenState.NeedsFlipping: OnFlipRequired?.Invoke(); break;
            case OvenState.Baked:         OnCakeBaked?.Invoke();    break;
            case OvenState.Burned:        OnCakeBurned?.Invoke();   break;
        }
    }
}
