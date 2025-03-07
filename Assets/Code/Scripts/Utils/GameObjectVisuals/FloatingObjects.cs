using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.PingPong(Time.time * speed, amplitude * 2) - amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
