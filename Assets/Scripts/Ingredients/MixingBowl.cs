using UnityEngine;
using System.Collections.Generic;

public class MixingBowl : MonoBehaviour
{
    [Header("Ingredientes Requeridos")]
    public List<IngredientType> requiredIngredients;

    [Header("Mezcla")]
    [Range(0f, 1f)]
    public float mixProgress;
    public float mixSpeed = 0.3f;
    public float mixVelocityThreshold = 1f;

    private List<IngredientType> addedIngredients = new List<IngredientType>();
    private GameObject currentMixer;
    private Vector3 lastMixerPosition;

    public bool HasAllIngredients => GetMissingIngredients().Count == 0;
    public bool IsMixingComplete => mixProgress >= 1f;

    public System.Action<IngredientType> OnIngredientAdded;
    public System.Action OnMixingComplete;

    void Update()
    {
        if (!HasAllIngredients || IsMixingComplete || currentMixer == null) return;

        float velocity = (currentMixer.transform.position - lastMixerPosition).magnitude / Time.deltaTime;

        if (velocity >= mixVelocityThreshold)
        {
            mixProgress = Mathf.Clamp01(mixProgress + mixSpeed * Time.deltaTime);

            if (IsMixingComplete)
                OnMixingComplete?.Invoke();
        }

        lastMixerPosition = currentMixer.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null && !ingredient.IsUsed)
        {
            AddIngredient(ingredient);
            return;
        }

        if (other.CompareTag("Mixer"))
        {
            currentMixer = other.gameObject;
            lastMixerPosition = currentMixer.transform.position;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mixer"))
            currentMixer = null;
    }

    void AddIngredient(Ingredient ingredient)
    {
        if (addedIngredients.Contains(ingredient.type)) return;

        addedIngredients.Add(ingredient.type);
        ingredient.Use();
        OnIngredientAdded?.Invoke(ingredient.type);
    }

    public bool HasIngredient(IngredientType type) => addedIngredients.Contains(type);

    public List<IngredientType> GetMissingIngredients()
    {
        var missing = new List<IngredientType>();
        foreach (var req in requiredIngredients)
        {
            if (!addedIngredients.Contains(req))
                missing.Add(req);
        }
        return missing;
    }
}
