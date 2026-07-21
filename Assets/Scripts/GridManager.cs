using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    private bool[,] gridData;
    private float offsetX;
    private float offsetY;

    void Start()
    {
        gridData = new bool[gridWidth, gridHeight];
        offsetX = (gridWidth * cellSize) / 2f - cellSize / 2f;
        offsetY = (gridHeight * cellSize) / 2f - cellSize / 2f;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = GridToWorldPosition(x, y);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";

                gridData[x, y] = false; // Başlangıçta tüm hücreler boş
            }
        }
    }

    // Grid koordinatını (x, y) dünya pozisyonuna çevirir
    public Vector3 GridToWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize - offsetX, y * cellSize - offsetY, 0);
    }

    // Dünya pozisyonunu en yakın grid koordinatına çevirir
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x + offsetX) / cellSize);
        int y = Mathf.RoundToInt((worldPosition.y + offsetY) / cellSize);
        return new Vector2Int(x, y);
    }

    // Verilen koordinat grid sınırları içinde mi?
    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    // Verilen hücre boş mu?
    public bool IsCellEmpty(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return false;
        return !gridData[x, y];
    }

    // Hücreyi doldur veya boşalt
    public void SetCellOccupied(int x, int y, bool occupied)
    {
        if (IsInsideGrid(x, y))
        {
            gridData[x, y] = occupied;
        }
    }

    // Grid'de boş bir hücre var mı kontrol eder
    public bool HasEmptyCell()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!gridData[x, y])
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Dolu olan satır/sütunları kontrol eder ve varsa temizler
    public void CheckAndClearLines()
    {
        int linesCleared = 0;

        // Dolu satırları bul
        for (int y = 0; y < gridHeight; y++)
        {
            bool rowFull = true;
            for (int x = 0; x < gridWidth; x++)
            {
                if (!gridData[x, y])
                {
                    rowFull = false;
                    break;
                }
            }

            if (rowFull)
            {
                ClearRow(y);
                linesCleared++;
            }
        }

        // Dolu sütunları bul
        for (int x = 0; x < gridWidth; x++)
        {
            bool columnFull = true;
            for (int y = 0; y < gridHeight; y++)
            {
                if (!gridData[x, y])
                {
                    columnFull = false;
                    break;
                }
            }

            if (columnFull)
            {
                ClearColumn(x);
                linesCleared++;
            }
        }

        if (linesCleared > 0 && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(linesCleared);
        }
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            gridData[x, y] = false;
            DestroyBlockAt(x, y);
        }
    }

    private void ClearColumn(int x)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            gridData[x, y] = false;
            DestroyBlockAt(x, y);
        }
    }

    private void DestroyBlockAt(int x, int y)
    {
        Vector3 worldPos = GridToWorldPosition(x, y);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPos);
        foreach (var col in colliders)
        {
            if (col.GetComponent<DraggableBlock>() != null)
            {
                Destroy(col.gameObject);
            }
        }
    }
}