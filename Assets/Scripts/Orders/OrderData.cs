using UnityEngine;

[CreateAssetMenu(menuName = "Cake Game/Order")]
public class OrderData : ScriptableObject
{
    public string orderName;
    public FillingType requiredFilling;
    public CreamType requiredCream;
    public int minToppings = 3;
    public float timeLimit = 120f;
    public float tipPrecisionThreshold = 90f;
}
