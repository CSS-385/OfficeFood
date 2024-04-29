using UnityEngine;

// todo

namespace Interact
{
    public class Interactor : MonoBehaviour
    {
        public Vector2 queryDirection = Vector2.down;
        public float queryRange = 0.25f;

        public Vector2 raycastDirection = Vector2.down;
        public float raycastDistance = 0.25f;

        public Interactable GetInteractable()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(position, queryDirection.normalized, queryRange, LayerMask.GetMask("Default"));
            Interactable interactable = hit.transform?.GetComponent<Interactable>();
            if (hit.transform != null)
            {
                return hit.transform.GetComponent<Interactable>();
            }
            return null;
        }
    }
}
