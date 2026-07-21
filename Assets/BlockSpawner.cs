using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public GridManager gridManager;
    public GameOverManager gameOverManager;

    public Vector3[] spawnPositions;

    private GameObject[] currentBlocks = new GameObject[3];

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
        GameObject block = Instantiate(blockPrefab, spawnPositions[slotIndex], Quaternion.identity);
        currentBlocks[slotIndex] = block;

        DraggableBlock draggable = block.GetComponent<DraggableBlock>();
        if (draggable != null)
        {
            draggable.spawner = this;
            draggable.spawnSlotIndex = slotIndex;
        }
    }

    public void OnBlockPlaced(int slotIndex)
    {
        currentBlocks[slotIndex] = null;

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
        if (gridManager != null && !gridManager.HasEmptyCell())
        {
            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver();
            }
        }
    }
}