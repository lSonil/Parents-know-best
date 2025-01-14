using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MatrixHandler : MonoBehaviour
{

    public List<Rows> matrix;
    public GameObject catMatrix;
    public GameObject catCell;
    public GameObject testCell;
    public Vector2 startOffset;
    public Vector2 adding;
    public bool testing = false;

    private void Awake()
    {
        testing = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GenerateCells()
    {
        if (testing)
        {
            foreach (Transform child in catMatrix.transform) 
            {
                if(child.gameObject.name!=catCell.name &&  child.gameObject.name!=testCell.name)
                    DestroyImmediate(child.gameObject); 
            }
        }

        float x = startOffset.x, y = startOffset.y;
        for (int i = 0; i < matrix.Count; i++)
        {
            Rows row = matrix[i];
            for (int j = 0; j < row.point.Length; j++)
            {
                row.point[j] = new Vector3(x, y, 0); // Set the value of each Vector2 element
                if(testing)
                {
                    GameObject tst = Instantiate(testCell.gameObject, catMatrix.transform);
                    testCell.transform.localPosition = row.point[j];
                }
                x += adding.x;
            }
            x = startOffset.x;
            y -= adding.y;
        }
    }

    public bool Blocked(int i, int j)
    {
        return matrix[i].blocked[j];
    }
    public Transform CatCell(int i, int j)
    {
        catCell.transform.localPosition = matrix[i].point[j];
        return catCell.transform;
    }
    public Transform CellPosition(int i, int j)
    {
        testCell.transform.localPosition = matrix[i].point[j];
        return testCell.transform;
    }
    public void SetCellBlockage(int i, int j, bool blocked=true)
    {
        matrix[i].blocked[j] = blocked;
    }
}
[Serializable]
public struct Rows
{
    public Vector2[] point;
    public bool[] blocked;
}
