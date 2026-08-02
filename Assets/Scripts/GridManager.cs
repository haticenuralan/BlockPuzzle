using System;
using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    // GridManager artık ScoreManager'ı hiç tanımıyor.
    // Sadece "şu kadar satır/sütun temizlendi" diye haber veriyor.
    public event Action<int> OnLinesCleared;

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

    // Verilen şekil (hücre listesi) grid'de herhangi bir yere yerleştirilebilir mi?
    public bool CanShapeFitAnywhere(Vector2Int[] shapeCells)
    {
        // Grid'deki her olası başlangıç pozisyonunu dene
        for (int startX = 0; startX < gridWidth; startX++)
        {
            for (int startY = 0; startY < gridHeight; startY++)
            {
                bool fits = true;

                // Şeklin tüm hücreleri bu pozisyondan itibaren sığıyor mu?
                foreach (var cell in shapeCells)
                {
                    int x = startX + cell.x;
                    int y = startY + cell.y;

                    if (!IsInsideGrid(x, y) || !IsCellEmpty(x, y))
                    {
                        fits = false;
                        break;
                    }
                }

                if (fits)
                {
                    return true; // En az bir yere sığıyor
                }
            }
        }

        return false; // Hiçbir yere sığmıyor
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

        if (linesCleared > 0)
        {
            OnLinesCleared?.Invoke(linesCleared);
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

        // Bu noktadaki hücre görselini bul ve flash'layıp yok et.
        SpriteRenderer sr = FindCellSpriteAt(worldPos);
        if (sr != null)
        {
            StartCoroutine(FlashAndDestroy(sr.gameObject));
        }
    }

    private SpriteRenderer FindCellSpriteAt(Vector3 worldPos)
    {
        DraggableBlock[] allBlocks = FindObjectsByType<DraggableBlock>(FindObjectsSortMode.None);

        foreach (var block in allBlocks)
        {
            foreach (Transform child in block.transform)
            {
                // Bu child hücresi, aradığımız grid pozisyonunda mı?
                if (Vector3.Distance(child.position, worldPos) < 0.1f)
                {
                    return child.GetComponent<SpriteRenderer>();
                }
            }
        }
        return null;
    }

    // Hücreyi kısa süre beyaz parlatıp sonra yok eder
    private IEnumerator FlashAndDestroy(GameObject cellObject)
    {
        SpriteRenderer sr = cellObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Destroy(cellObject);
            yield break;
        }

        Color originalColor = sr.color;
        float flashDuration = 0.12f;
        float t = 0f;

        // Beyaza doğru parla
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            sr.color = Color.Lerp(originalColor, Color.white, t / flashDuration);
            yield return null;
        }

        Destroy(cellObject);
    }
}