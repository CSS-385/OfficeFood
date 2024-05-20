using Unity.Mathematics;
using UnityEngine;

namespace OfficeFood.Interact
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField, PostNormalize]
        private Vector2 _queryDirection = Vector2.down;
        public Vector2 queryDirection
        {
            get
            {
                return _queryDirection;
            }
            set
            {
                if (value.sqrMagnitude > 0.0f)
                {
                    _queryDirection = value.normalized;
                }
            }
        }

        [SerializeField, Min(0.0f)]
        private float _queryDistance = 0.375f;
        public float queryDistance
        {
            get
            {
                return _queryDistance;
            }
            set
            {
                _queryDistance = Mathf.Max(value, 0.0f);
            }
        }

        [SerializeField]
        private bool _canHighlight = false;// If this can highlight Interactables (should only be set once in Inspector).

        private Interactable _queryInteractable = null;
        private Interactable queryInteractable
        {
            get
            {
                return _queryInteractable;
            }
            set
            {
                if (_queryInteractable != value)
                {
                    if (_queryInteractable != null)
                    {
                        if (_canHighlight && (_queryInteractable.highlightable != null))
                        {
                            _queryInteractable.highlightable.DecrementHighlighterCount();
                        }
                    }
                    _queryInteractable = value;
                    if (_queryInteractable != null)
                    {
                        if (_canHighlight && (_queryInteractable.highlightable != null))
                        {
                            _queryInteractable.highlightable.IncrementHighlighterCount();
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(position, _queryDirection.normalized, _queryDistance, LayerMask.GetMask("Default"));
            queryInteractable = hit.collider?.transform?.GetComponent<Interactable>();
        }

        public bool Interact()
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            if (_queryInteractable != null)
            {
                _queryInteractable.Interacted?.Invoke();
                return true;
            }
            return false;
        }
    }
}
