using UnityEngine;

// Class that adds Z position to Y (negatives ignored)
public class OffsetZ : MonoBehaviour
{
    private float _positionZ = 0.0f;
    private void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + transform.position.z - _positionZ,
            transform.position.z);
        _positionZ = transform.position.z;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + transform.position.z - _positionZ,
            transform.position.z);
        _positionZ = transform.position.z;
    }
}
