using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Drawer : MonoBehaviour
{
    [Header("Interaction")] [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable handle;

    public Rigidbody rigidBody;

    [Header("Direction")] [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    private Vector3 _grabPosition = Vector3.zero;
    private float _startingPercentage;
    private float _currentPercentage;

    protected virtual void OnEnable()
    {
        handle.selectEntered.AddListener(StoreGrabInfo);
    }

    protected virtual void OnDisable()
    {
        handle.selectEntered.RemoveListener(StoreGrabInfo);
    }

    private void StoreGrabInfo(SelectEnterEventArgs args)
    {
        /*Update the starting position, this prevents the drawer from jumping to the hand.*/
        _startingPercentage = _currentPercentage;

        /*Store starting position for pull direction*/
        _grabPosition = args.interactorObject.transform.position;
    }

    private void Update()
    {
        /*If the handle of the drawer is selected, update the drawer*/
        if (handle.isSelected)
        {
            UpdateDrawer();
        }
    }

    private void UpdateDrawer()
    {
        /*From the starting percentage, apply the difference from each update*/
        var newPercentage = _startingPercentage + FindPercentageDifference();

        /*We then use a lerp to find the appropriate position between the start/endXrOriginPosition points.
        Allowing the percentage value to be unclamped gives us a more realistic response
        when pulling beyond the start/endXrOriginPosition positions.*/
        rigidBody.MovePosition(Vector3.Lerp(start.position, end.position, newPercentage));

        /*We clamp after the percentage has been applied to keep the current percentage valid
        for other uses.*/
        _currentPercentage = Mathf.Clamp01(newPercentage);
    }

    private float FindPercentageDifference()
    {
        /*Find the directions for the xrOrigin's pull direction and the target direction.*/
        var handPosition = handle.interactorsSelecting[0].transform.position;
        var pullDirection = handPosition - _grabPosition;
        var targetDirection = end.position - start.position;

        /*Store length, then normalize target direction for use in Vector3.Dot*/
        var length = targetDirection.magnitude;
        targetDirection.Normalize();

        /*Typically, we use two normalized vectors for Vector3.Dot. We'll be leaving one of the
        directions with its original length. Thus, we get an actual combination of the distance/direction
        similarity, then dividing by the length, gives us a value between 0 and 1.*/
        return Vector3.Dot(pullDirection, targetDirection) / length;
    }

    private void OnDrawGizmos()
    {
        /*Shows the general direction of the interaction*/
        if (start && end)
            Gizmos.DrawLine(start.position, end.position);
    }
}