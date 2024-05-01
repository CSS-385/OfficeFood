using UnityEngine;

namespace OfficeFood.Interact
{
    public class Interactor : MonoBehaviour
    {
        public Vector2 queryDirection = Vector2.down;
        public float queryRange = 0.25f;

        public void Interact()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(position, queryDirection.normalized, queryRange, LayerMask.GetMask("Default"));
            Interactable interactable = hit.transform?.GetComponent<Interactable>();
            interactable.Interacted.Invoke();
        }
    }
}
