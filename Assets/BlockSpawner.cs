using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Tooltip("Tek bir kare hücrenin görsel prefabı")]
    public GameObject cellVisualPrefab;

    [Tooltip("Spawn edilebilecek blok şekilleri")]
    public BlockShapeData[] availableShapes;

    public GridManager gridManager;
    public GameOverManager gameOverManager;

    public Vector3[] spawnPositions;

    private GameObject[] currentBlocks = new GameObject[3];
    private BlockShapeData[] currentShapes = new BlockShapeData[3];

    void Start()
    {
        SpawnAllBlocks();
    }

    void SpawnAllBlocks()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            SpawnBlockAt(i);
        }
    }

    void SpawnBlockAt(int slotIndex)
    {
        // Rastgele bir şekil seç
        BlockShapeData shape = availableShapes[Random.Range(0, availableShapes.Length)];

        // Ana (parent) obje: sürükleme ve mantık bunda olacak
        GameObject blockParent = new GameObject("Block_" + shape.name);
        blockParent.transform.position = spawnPositions[slotIndex];

        // Şeklin her hücresi için bir görsel kare oluştur (child olarak)
        foreach (var cell in shape.cells)
        {
            GameObject cellVisual = Instantiate(cellVisualPrefab, blockParent.transform);
            cellVisual.transform.localPosition = new Vector3(cell.x, cell.y, 0);

            // Şeklin rengini hücre görseline uygula
            SpriteRenderer cellSprite = cellVisual.GetComponent<SpriteRenderer>();
            if (cellSprite != null)
            {
                cellSprite.color = shape.shapeColor;
                cellSprite.sortingOrder = 1; // Bloklar her zaman grid hücrelerinin üstünde görünsün
            }

            // Child'lardaki eski script/collider'ları temizle:
            // sürükleme ana objeden yönetilecek, child'lar sadece görsel
            DraggableBlock childDraggable = cellVisual.GetComponent<DraggableBlock>();
            if (childDraggable != null) Destroy(childDraggable);

            Collider2D childCollider = cellVisual.GetComponent<Collider2D>();
            if (childCollider != null) Destroy(childCollider);
        }

        // Ana objeye sürükleme scripti ve collider ekle
        DraggableBlock draggable = blockParent.AddComponent<DraggableBlock>();
        draggable.shapeCells = shape.cells;
        draggable.spawner = this;
        draggable.spawnSlotIndex = slotIndex;

        // Collider: şeklin tüm hücrelerini kapsayacak şekilde
        BoxCollider2D collider = blockParent.AddComponent<BoxCollider2D>();
        Bounds bounds = CalculateShapeBounds(shape.cells);
        collider.offset = bounds.center;
        collider.size = bounds.size;

        blockParent.AddComponent<BlockAnimator>();

        currentBlocks[slotIndex] = blockParent;
        currentShapes[slotIndex] = shape;
    }

    // Şeklin hücrelerini kapsayan sınırları hesapla (collider için)
    private Bounds CalculateShapeBounds(Vector2Int[] cells)
    {
        Vector2Int min = cells[0];
        Vector2Int max = cells[0];

        foreach (var cell in cells)
        {
            min = Vector2Int.Min(min, cell);
            max = Vector2Int.Max(max, cell);
        }

        Vector3 center = new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, 0);
        Vector3 size = new Vector3(max.x - min.x + 1, max.y - min.y + 1, 1);

        return new Bounds(center, size);
    }

    public void OnBlockPlaced(int slotIndex)
    {
        currentBlocks[slotIndex] = null;
        currentShapes[slotIndex] = null;

        bool allEmpty = true;
        foreach (var b in currentBlocks)
        {
            if (b != null)
            {
                allEmpty = false;
                break;
            }
        }

        if (allEmpty)
        {
            SpawnAllBlocks();
        }

        CheckGameOver();
    }

    void CheckGameOver()
    {
        if (gridManager == null || gameOverManager == null) return;

        // Elimizdeki (henüz yerleştirilmemiş) şekillerden herhangi biri sığıyor mu?
        bool anyShapeFits = false;
        foreach (var shape in currentShapes)
        {
            if (shape != null && gridManager.CanShapeFitAnywhere(shape.cells))
            {
                anyShapeFits = true;
                break;
            }
        }

        // Hiçbiri sığmıyorsa oyun biter
        if (!anyShapeFits)
        {
            gameOverManager.TriggerGameOver();
        }
    }
}