using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource = null;

    [SerializeField]
    private float _delay = 0.0f;

    private void OnEnable()
    {
        if (_audioSource != null)
        {
            _audioSource.PlayDelayed(_delay);
        }
    }
}
