using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] public Vector2Int Coordinate;
    [SerializeField] public bool Accessible;
    [SerializeField] public CellHigh CellHigh;

    public void OnHighChanged()
    {
        Vector3 position = transform.position;
        switch (CellHigh)
        {
            case CellHigh.LOW:
                position.y = -1f;
                break;
            case CellHigh.MEDIUM:
                position.y = 0f;
                break;
            case CellHigh.HIGH:
                position.y = 1f;
                break;
            default:
                position.y = -1f;
                break;
        }
        transform.position = position;
    }
}
