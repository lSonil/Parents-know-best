using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Dialog 
{
    [SerializeField] List<DialogLine> lines;
    public List<DialogLine> Lines
    {
        get { return lines; }
    }
}

[System.Serializable] // Make it serializable to show up in the Inspector
public struct DialogLine
{
    public string lineText;  // The dialog line text
    public int lineSpeed; // speed of dialog
    public int linePause; // length of pause between dialogs
    public bool keepLine; // keep writeing in same dialog

}