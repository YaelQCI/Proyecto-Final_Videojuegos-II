using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Pedidos")]
    public OrderData[] orders;
    public int ordersToWin = 3;

    [Header("Penalizacion por tiempo agotado")]
    public int timeoutPenalty = 50;

    public DeliveryStation deliveryStation;

    public OrderData CurrentOrder => orders[currentOrderIndex];
    public int Score { get; private set; }
    public int CompletedOrders { get; private set; }
    public float TimeRemaining { get; private set; }

    private int currentOrderIndex = 0;

    // (scoreGained, coins, hasTip)
    public System.Action<OrderData> OnOrderStarted;
    public System.Action<int, int, bool> OnOrderCompleted;
    public System.Action OnOrderFailed;
    public System.Action<float> OnTimeUpdated;
    public System.Action<int> OnScoreUpdated;

    void Start()
    {
        deliveryStation.OnBoxDelivered += HandleDelivery;
        StartOrder();
    }

    void OnDestroy()
    {
        if (deliveryStation != null)
            deliveryStation.OnBoxDelivered -= HandleDelivery;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        TimeRemaining -= Time.deltaTime;
        OnTimeUpdated?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0)
        {
            Score = Mathf.Max(0, Score - timeoutPenalty);
            OnScoreUpdated?.Invoke(Score);
            OnOrderFailed?.Invoke();
            NextOrder();
        }
    }

    void StartOrder()
    {
        TimeRemaining = CurrentOrder.timeLimit;
        OnOrderStarted?.Invoke(CurrentOrder);
    }

    void HandleDelivery(CakeBox box)
    {
        if (!ValidateDelivery(box))
        {
            OnOrderFailed?.Invoke();
            NextOrder();
            return;
        }

        float precision = box.CakeInside.Precision;
        bool hasTip = precision >= CurrentOrder.tipPrecisionThreshold;
        int scoreGained = 100 + Mathf.RoundToInt(precision);
        int coins = 50 + (hasTip ? 30 : 0);

        Score += scoreGained;
        CompletedOrders++;
        OnOrderCompleted?.Invoke(scoreGained, coins, hasTip);
        OnScoreUpdated?.Invoke(Score);

        NextOrder();
    }

    bool ValidateDelivery(CakeBox box)
    {
        CakeFillingArea filling = box.CakeInside.GetComponent<CakeFillingArea>();
        if (filling != null && filling.AppliedFilling != CurrentOrder.requiredFilling)
            return false;

        if (box.CakeInside.AppliedCream != CurrentOrder.requiredCream)
            return false;

        if (box.CakeInside.ToppingsPlaced < CurrentOrder.minToppings)
            return false;

        return true;
    }

    void NextOrder()
    {
        currentOrderIndex++;

        if (CompletedOrders >= ordersToWin)
        {
            GameManager.Instance.SetState(GameManager.GameState.Victory);
            return;
        }

        if (currentOrderIndex >= orders.Length)
        {
            GameManager.Instance.SetState(GameManager.GameState.GameOver);
            return;
        }

        StartOrder();
    }
}
