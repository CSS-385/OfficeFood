using UnityEngine;

public class WASD : MonoBehaviour
{
    public float speed = 10;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speed * Time.deltaTime * Vector3.up;
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            transform.position += speed * Time.deltaTime * Vector3.down;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += speed * Time.deltaTime * Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
        }
    }
}
