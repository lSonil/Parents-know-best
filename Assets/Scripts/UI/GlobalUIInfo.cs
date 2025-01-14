using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalUIInfo : MonoBehaviour
{
    public static GlobalUIInfo i;

    public GameObject menu;
    public TextMeshProUGUI clockText;
    
    public GameObject interactionKey;
    public List<TextMeshProUGUI> itemsDisplay;
    public GameObject noteWindow;
    public NoteHandler note;
    public GameObject useKey;
    public GameObject dropKey;
    public GameObject itemPrompt;
    public TextMeshProUGUI itemText;
    public DialogManager dialogManager;
    public CinemachineVirtualCamera vc;
    public RGBShiftEffect shift;

    public bool pause;
    public bool lightOn;

    public GameObject screen;

    private void Start()
    {
        screen.SetActive(false);
    }

    public void GameOver()
    {
        StartCoroutine(FadeTo());
    }

    private IEnumerator FadeTo()
    {
        yield return new WaitForSeconds(3);
        screen.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("GameScene");
    }

    private void Awake()
    {
        i = this;
        noteWindow.SetActive(false);
        menu.SetActive(false);
        itemPrompt.SetActive(false);
        useKey.SetActive(false);
        dropKey.SetActive(false);
        interactionKey.SetActive(false);
    }

    public void UpdateUI()
    {
        foreach (TextMeshProUGUI text in itemsDisplay)
        {
            text.text = "";
        }

        for (int i = 1; i < Inventory.i.items.Length; i++) 
        {
            itemsDisplay[i-1].text = Inventory.i.items[i].name;
        }
    }
    public void DisplayItem(string name)
    {
        itemPrompt.SetActive(true);
        itemText.text = name;

        StartCoroutine(StopItemPrompt());

    }
    public void Update()
    {
        SearchAI[] allDerivedClasses = FindObjectsByType<SearchAI>(FindObjectsSortMode.None);
        bool isHunted=false;
        foreach (SearchAI search in allDerivedClasses)
        {
            if (search.hunt)
                isHunted = true;
        }
        shift.on = isHunted;
    }
    // Coroutine pentru fade-in și fade-out
    private IEnumerator StopItemPrompt()
    {
        yield return new WaitForSeconds(3f);
        itemPrompt.SetActive(false);
    }
    public void ShowMenu(bool stop = true)
    {
        menu.SetActive(stop);
        Time.timeScale = stop ? 0 : 1;
        pause = stop;

        itemPrompt.SetActive(false);
        useKey.SetActive(false);
        dropKey.SetActive(false);
        interactionKey.SetActive(false);
    }
    public void ShowNote(TMP_Text text =null, bool stop = true)
    {
        if (text !=null)
            note.textMeshPro.text = text.text;
        noteWindow.SetActive(stop);
        pause = stop;
        note.UpdateNote();
        Time.timeScale = stop ? 0 : 1;
    }

    public void ShowKey(bool status)
    {
        print(status);
        if(noteWindow.activeInHierarchy)
            interactionKey.SetActive(false);
        else
            interactionKey.SetActive(status);
    }

    public void UseKey(bool status)
    {
        if (noteWindow.activeInHierarchy)
            useKey.SetActive(false);
        else
            useKey.SetActive(status);
    }

    public void DropKey(bool status)
    {
        if (noteWindow.activeInHierarchy)
            dropKey.SetActive(false);
        else
            dropKey.SetActive(status);
    }
}
