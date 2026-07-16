using UnityEngine;

public class DraggableBlock : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 startPosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private GridManager gridManager;

    // Bu blok şu an hangi grid hücresinde duruyor (henüz yerleştirilmediyse -1,-1)
    private int currentGridX = -1;
    private int currentGridY = -1;

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

        // Eğer daha önce bir hücrede duruyorsa, oradan çık (geçici olarak boşalt)
        if (currentGridX != -1)
        {
            gridManager.SetCellOccupied(currentGridX, currentGridY, false);
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
        Vector2Int gridPos = gridManager.WorldToGridPosition(transform.position);

        if (gridManager.IsInsideGrid(gridPos.x, gridPos.y) && gridManager.IsCellEmpty(gridPos.x, gridPos.y))
        {
            // Geçerli ve boş hücre: bloğu oraya sabitle
            transform.position = gridManager.GridToWorldPosition(gridPos.x, gridPos.y);
            gridManager.SetCellOccupied(gridPos.x, gridPos.y, true);
            currentGridX = gridPos.x;
            currentGridY = gridPos.y;
        }
        else
        {
            // Geçersiz ya da dolu hücre: eski konuma geri dön
            transform.position = startPosition;

            if (currentGridX != -1)
            {
                gridManager.SetCellOccupied(currentGridX, currentGridY, true);
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