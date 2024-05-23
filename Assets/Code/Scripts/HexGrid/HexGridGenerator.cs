using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexGridGenerator : MonoBehaviour
{
    [field: SerializeField] public HexGrid HexGrid { get; private set; }
    [SerializeField] private Material AlternativeMaterial;

    private void Awake()
    {
        if (HexGrid == null)
        {
            HexGrid = GetComponentInParent<HexGrid>();
        }
        if (HexGrid == null)
        {
            Debug.LogError("HexGridGenerator could not find HexGrid component in its parent or itself");
        }
    }

    public void CreateHexMesh()
    {
        CreateHexMesh(HexGrid.Width, HexGrid.Height, HexGrid.HexSize, HexGrid.HexOrientation);
    }

    private void CreateHexMesh(HexGrid hexGrid)
    {
        this.HexGrid = hexGrid;
        CreateHexMesh(HexGrid.Width, HexGrid.Height, HexGrid.HexSize, HexGrid.HexOrientation);
    }

    private void CreateHexMesh(int width, int height, float hexSize, HexOrientation orientation)
    {
        var rotation = Quaternion.identity;
        ClearHexGridMesh();
        if (orientation.Equals(HexOrientation.FlatTop))
        {
            rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 centerPosition = HexMetrics.Center(hexSize, x, z, orientation);
                GameObject newHex = Instantiate<GameObject>(HexGrid.HexPrefab, centerPosition, rotation, HexGrid.transform);

                newHex.transform.name = "Cell[" + x + ";" + z + "]";
                newHex.GetComponent<Cell>().Coordinate = new Vector2Int(x, z);
                newHex.GetComponent<Cell>().Accessible = true;
                newHex.GetComponent<Cell>().CellHigh = CellHigh.LOW;
                CellVisual childVisual = newHex.transform.GetComponentInChildren<CellVisual>();
                childVisual.transform.localScale = Vector3.one * hexSize / 2.68f;

                if (x % 2 == 0)
                {
                    childVisual.GetComponent<Renderer>().material = AlternativeMaterial;
                }

            }
        }
    }
    public void ClearHexGridMesh()
    {

        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
        if (cells != null)
        {
            foreach (GameObject cell in cells)
            {
                DestroyImmediate(cell);
            }
        }
    }

    public void GenerateRandomHigh()
    {
        int childCount = HexGrid.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Cell child = HexGrid.transform.GetChild(i).GetComponent<Cell>();
            child.CellHigh = (CellHigh)Random.Range(0, 3);
            child.OnHighChanged();
        }
    }
}
