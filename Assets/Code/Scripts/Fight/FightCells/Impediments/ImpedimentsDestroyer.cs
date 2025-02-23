using UnityEngine;

public class ImpedimentsDestroyer : MonoBehaviour
{
    public float delay = 2f;

    private void Start()
    {
        Destroy(gameObject, delay);
    }
}


