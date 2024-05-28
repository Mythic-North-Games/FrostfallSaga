using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public float HexSize { get; private set; }
    [field: SerializeField] public GameObject HexPrefab { get; private set; }
    [field: SerializeField] public HexOrientation HexOrientation { get; private set; }

    private void OnDrawGizmos()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 centerPosition = HexMetrics.Center(HexSize, x, z, HexOrientation) + transform.position;

                for (int s = 0; s < HexMetrics.Corners(HexSize, HexOrientation).Length; s++)
                {
                    Gizmos.DrawLine(
                        centerPosition + HexMetrics.Corners(HexSize, HexOrientation)[s % 6],
                        centerPosition + HexMetrics.Corners(HexSize, HexOrientation)[(s + 1) % 6]
                     );
                }
            }
        }
    }

    private void OnEnable()
    {
        MouseController.Instance.OnLeftMouseClick += OnClickLeft;
    }
    private void OnDisable()
    {
        MouseController.Instance.OnLeftMouseClick -= OnClickLeft;
    }
    private void OnClickLeft(RaycastHit hit)
    {
        Cell parent = hit.transform.GetComponentInParent<Cell>();
        Debug.Log("[X=" + parent.Coordinate.x + ";" + parent.Coordinate.y + "]");
    }

}
