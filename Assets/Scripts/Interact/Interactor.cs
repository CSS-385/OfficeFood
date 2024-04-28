using System;
using UnityEngine;

// todo

public class Interactor : MonoBehaviour
{
    // Raycasts every fixed frame for an interactable. Fetch using GetInteractable().
    // note: Physics raycasts do not detect colliders inside origin
    public Vector2 raycastDirection = Vector2.down;
    public float raycastDistance = 0.25f;

    // Raycasts (should be called from FixedUpdate)
    public Interactable GetInteractable()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        LayerMask layerMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.Raycast(position, raycastDirection, raycastDistance, layerMask);
        if (hit.transform != null)
        {
            return hit.transform.GetComponent<Interactable>();
        }
        return null;
    }


}
