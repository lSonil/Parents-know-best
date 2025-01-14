using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;


public class DialogManager : MonoBehaviour
{
    [SerializeField] public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    int curentLine = 0;
    Dialog dialog;
    bool isTyping;
    public bool dialogOn;

    private void Start()
    {
        dialogBox.SetActive(false);
    }
    public IEnumerator ShowDialog(Dialog text)
    {
        dialogText.text = "";
        dialogBox.SetActive(true);
        yield return new WaitForEndOfFrame();
        dialogOn = true;
        dialog = text;
        StartCoroutine(TypeDialog(text.Lines[0]));
    }

    private void Update()
    {
        if (dialogOn == true)
        {
            HandleUpdate();
        }
    }
    public void HandleUpdate()
    {
        if (isTyping == false)
        {
            curentLine++;
            if (curentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[curentLine]));
            }
            else
            {
                dialogOn = false;
                curentLine = 0;
                dialogBox.SetActive(false);
            }
        }
    }
    public IEnumerator TypeDialog(DialogLine line)
    {
        isTyping = true;
        if(!line.keepLine)
            dialogText.text = "";
        foreach (var letter in line.lineText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / line.lineSpeed);
        }

        yield return new WaitForSeconds(line.linePause);
        isTyping = false;
    }
}
