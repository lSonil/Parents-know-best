using TMPro;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{

    public TMP_Text textMeshPro; // Asociază componenta TextMeshPro din Inspector.
    public RectTransform rectTransform;
    float offsetY = 84;
    int maxLineCount;
    int currentLineCount;
    bool update;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateNote()
    {
        textMeshPro.ForceMeshUpdate();
        maxLineCount = textMeshPro.textInfo.lineCount;
        Vector2 size = rectTransform.sizeDelta;
        size.y = maxLineCount * offsetY;
        rectTransform.sizeDelta = size;

    }

    // Update is called once per frame
    void Update()
    {
        if (maxLineCount > 11)
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentLineCount++;
                update = true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentLineCount--;
                update = true;
            }

        if (update)
        {
            currentLineCount = Mathf.Clamp(currentLineCount, 0, maxLineCount - 11);
            transform.localPosition = new Vector3(0, offsetY * currentLineCount+ 540, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GlobalUIInfo.i.ShowNote(null, false);
        }
    }
}
