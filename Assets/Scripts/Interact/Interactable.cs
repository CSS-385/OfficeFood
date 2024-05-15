using OfficeFood.Highlight;
using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Interact
{
    public class Interactable : MonoBehaviour
    {
        public UnityEvent Interacted = new UnityEvent();
        public Highlightable highlightable = null;// Used if a Interactor detects this.
    }
}
