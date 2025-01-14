using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatrixHandler))]
public class MatrixManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MatrixHandler manager = (MatrixHandler)target;

        if (GUILayout.Button("Generate Cells"))
        {
            manager.GenerateCells();
        }
    }
}
