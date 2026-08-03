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

    // Blok grid'e kalıcı yerleştirildi mi? (yerleştirilince artık taşınamaz)
    private bool isPlaced = false;

    void Start()
    {
        mainCamera = Camera.main;
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void OnMouseDown()
    {
        // Blok zaten yerleştirildiyse, artık taşınamaz
        if (isPlaced) return;

        offset = transform.position - GetMouseWorldPosition();
        startPosition = transform.position;
        isDragging = true;
    }

    void OnMouseUp()
    {
        if (isPlaced) return;

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
        // Referans hücrenin (ilk child, yani şeklin (0,0) noktası) dünya pozisyonunu al
        Vector3 referenceCellWorldPos = transform.position;
        if (transform.childCount > 0)
        {
            referenceCellWorldPos = transform.GetChild(0).position;
        }
        Vector2Int basePos = gridManager.WorldToGridPosition(referenceCellWorldPos);

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
            // Referans hücrenin hedef pozisyonu ile mevcut pozisyonu arasındaki farkı hesapla,
            // tüm bloğu buna göre kaydır (böylece child'lar tam hücrelere oturur)
            Vector3 targetReferencePos = gridManager.GridToWorldPosition(basePos.x, basePos.y);
            Vector3 currentReferencePos = transform.childCount > 0 ? transform.GetChild(0).position : transform.position;
            Vector3 shift = targetReferencePos - currentReferencePos;
            transform.position += shift;

            // Tüm hücreleri doldur
            occupiedCells = new Vector2Int[shapeCells.Length];
            for (int i = 0; i < shapeCells.Length; i++)
            {
                int x = basePos.x + shapeCells[i].x;
                int y = basePos.y + shapeCells[i].y;
                gridManager.SetCellOccupied(x, y, true);
                occupiedCells[i] = new Vector2Int(x, y);
            }

            // Yerleşme animasyonunu oynat
            BlockAnimator animator = GetComponent<BlockAnimator>();
            if (animator != null)
            {
                animator.PlayPlaceAnimation();
            }
            // Yerleşme sesini çal
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlaceSound();
            }
            gridManager.CheckAndClearLines();

            isPlaced = true; // Blok artık kilitli, taşınamaz

            if (spawner != null && spawnSlotIndex != -1)
            {
                spawner.OnBlockPlaced(spawnSlotIndex);
                spawnSlotIndex = -1;
            }
        }
        else
        {
            // Geçersiz: eski konuma dön
            transform.position = startPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}