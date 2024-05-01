using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Interact
{
    public class Interactable : MonoBehaviour
    {
        public UnityEvent Interacted = new UnityEvent();
    }
}
