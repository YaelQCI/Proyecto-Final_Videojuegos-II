using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToggleRay : MonoBehaviour
{
    // Referencia al componente que ya tienes
    public NearFarInteractor interactor;
    
    // Referencia a la acción de apretar el gatillo (T)
    public InputActionReference triggerAction;

    void Update()
    {
        // Detectamos si la tecla T (Trigger) está siendo presionada
        bool isPressed = triggerAction.action.IsPressed();

        // Activamos el rayo solo mientras se presione el botón
        interactor.enableFarCasting = isPressed;
    }
}