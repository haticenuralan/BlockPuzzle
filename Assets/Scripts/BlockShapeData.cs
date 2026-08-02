using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockShape", menuName = "BlockPuzzle/Block Shape")]
public class BlockShapeData : ScriptableObject
{
    [Tooltip("Bu şeklin kapladığı hücreler, (0,0) şeklin referans noktası kabul edilerek")]
    public Vector2Int[] cells = new Vector2Int[] { new Vector2Int(0, 0) };

    [Tooltip("Bu şeklin rengi (opsiyonel, ileride kullanılabilir)")]
    public Color shapeColor = Color.white;
}