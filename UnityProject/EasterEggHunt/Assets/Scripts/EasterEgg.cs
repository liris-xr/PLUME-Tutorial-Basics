using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class EasterEgg : MonoBehaviour
{
    public Quest quest;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;

    private void Start()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        _interactable.hoverEntered.AddListener(ObjectPickedUp);
    }

    private void ObjectPickedUp(HoverEnterEventArgs args)
    {
        quest.OnEggPickUp(this);
    }
}