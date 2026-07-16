using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        
        float offsetX = (gridWidth * cellSize) / 2f - cellSize / 2f;
        float offsetY = (gridHeight * cellSize) / 2f - cellSize / 2f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * cellSize - offsetX, y * cellSize - offsetY, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";
            }
        }
    }
}