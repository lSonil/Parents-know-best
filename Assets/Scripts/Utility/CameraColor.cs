using UnityEngine;

[ExecuteInEditMode]
public class CameraColor : MonoBehaviour
{
    public Material material;
    public Camera targetCamera;

    void Update()
    {
        ApplyMaterialColorToCamera();
    }

    void ApplyMaterialColorToCamera()
    {
        if (material != null && targetCamera != null)
        {
            // Get the "TargetColor" parameter from the material
            if (material.HasProperty("_TargetColor"))
            {
                Color targetColor = material.GetColor("_TargetColor");
                if(targetColor!= targetCamera.backgroundColor)
                // Apply the color to the camera's background color
                targetCamera.backgroundColor = targetColor;
            }
            else
            {
                Debug.LogWarning("Material does not have an _TargetColor property.");
            }
        }
    }
}