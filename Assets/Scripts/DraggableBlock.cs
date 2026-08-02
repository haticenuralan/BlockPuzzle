using UnityEngine;

public class DraggableBlock : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 startPosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private GridManager gridManager;

    public BlockSpawner spawner;
    public int spawnSlotIndex = -1;

    // Bu bloğun şekli: hangi hücreleri kaplıyor (referans noktasına göre)
    public Vector2Int[] shapeCells = new Vector2Int[] { new Vector2Int(0, 0) };

    // Bu blok yerleştirildiğinde kapladığı grid hücreleri (boşsa yerleştirilmemiş)
    private Vector2Int[] occupiedCells = null;

    void Start()
    {
        mainCamera = Camera.main;
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        startPosition = transform.position;
        isDragging = true;

        // Daha önce yerleştirildiyse, kapladığı hücreleri geçici olarak boşalt
        if (occupiedCells != null)
        {
            foreach (var cell in occupiedCells)
            {
                gridManager.SetCellOccupied(cell.x, cell.y, false);
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        TrySnapToGrid();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private void TrySnapToGrid()
    {
        // Ana objenin (referans hücrenin) hedef grid pozisyonu
        Vector2Int basePos = gridManager.WorldToGridPosition(transform.position);

        // Şeklin TÜM hücreleri grid içinde ve boş mu kontrol et
        bool canPlace = true;
        foreach (var cell in shapeCells)
        {
            int x = basePos.x + cell.x;
            int y = basePos.y + cell.y;

            if (!gridManager.IsInsideGrid(x, y) || !gridManager.IsCellEmpty(x, y))
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace)
        {
            // Geçerli: bloğu yerleştir, tüm hücreleri doldur
            transform.position = gridManager.GridToWorldPosition(basePos.x, basePos.y);

            occupiedCells = new Vector2Int[shapeCells.Length];
            for (int i = 0; i < shapeCells.Length; i++)
            {
                int x = basePos.x + shapeCells[i].x;
                int y = basePos.y + shapeCells[i].y;
                gridManager.SetCellOccupied(x, y, true);
                occupiedCells[i] = new Vector2Int(x, y);
            }

            gridManager.CheckAndClearLines();

            if (spawner != null && spawnSlotIndex != -1)
            {
                spawner.OnBlockPlaced(spawnSlotIndex);
                spawnSlotIndex = -1;
            }
        }
        else
        {
            // Geçersiz: eski konuma dön, önceki hücreleri geri doldur
            transform.position = startPosition;

            if (occupiedCells != null)
            {
                foreach (var cell in occupiedCells)
                {
                    gridManager.SetCellOccupied(cell.x, cell.y, true);
                }
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}