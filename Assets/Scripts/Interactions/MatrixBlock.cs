using Unity.VisualScripting;
using UnityEngine;

public class MatrixBlock : MonoBehaviour
{
    public Vector2[] blockCell;

    public void Block(bool block)
    {
        MatrixHandler matrix = FindFirstObjectByType<MatrixHandler>();

        foreach (Vector2 cell in blockCell)
        {
            matrix.SetCellBlockage((int)cell.x, (int)cell.y, block);
        }
    }
}
