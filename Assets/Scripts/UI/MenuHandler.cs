using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public RectTransform[] buttons;
    public RectTransform[] options;
    public AudioMixer mixer;
    public RectTransform[] volumeSliders;
    public RectTransform iconMenu;
    public RectTransform iconOption;
    public GameObject instructions;
    public GameObject option;
    public GameObject menu;
    public int currentMenu = 0;
    public int currentIndex = 0;
    public float currentVolume = 0;
    public bool mainManu = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame

    private void Start()
    {
        option.SetActive(false);
        instructions.SetActive(false);
        menu.SetActive(true);
        UpdateMenu();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentIndex++;
            switch (currentMenu)
            {
                case 0:
                    UpdateMenu();
                    break;
                case 1:
                    UpdateOptions();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentIndex--;
            switch (currentMenu)
            {
                case 0:
                    UpdateMenu();
                    break;
                case 1:
                    UpdateOptions();
                    break;
            }
        }

        if (currentMenu == 1 && currentIndex!=4)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentVolume+=10;
                volumeSliders[currentIndex].gameObject.GetComponent<AudioSource>().Play();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentVolume-=10;
                volumeSliders[currentIndex].gameObject.GetComponent<AudioSource>().Play();
            }

            currentVolume = Mathf.Clamp(currentVolume, -80, 20);

            switch (currentIndex)
            {
                case 0:
                    mixer.SetFloat("masterVolume", currentVolume);
                    break;
                case 1:
                    mixer.SetFloat("musicVolume", currentVolume);
                    break;
                case 2:
                    mixer.SetFloat("sfxVolume", currentVolume);
                    break;
                case 3:
                    mixer.SetFloat("vfxVolume", currentVolume);
                    break;
            }
            AdjustSlider(currentIndex, currentVolume);


        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentMenu)
            {
                case 0:
                    MenuButtonPress();
                    break;
                case 1:
                    OptionsButtonPress();
                    break;
                case 2:
                    InstructionsButtonPress();
                    break;
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!mainManu)
            {
                GlobalUIInfo.i.ShowMenu(currentMenu != 0);
            }
            currentMenu = 0;
            currentIndex = 0;
            instructions.SetActive(false);
            option.SetActive(false);
            menu.SetActive(true);
            UpdateMenu();
        }
    }

    public void UpdateMenu()
    {
        if (currentIndex < 0)
            currentIndex = buttons.Length - 1;

        else if (currentIndex > buttons.Length - 1)
            currentIndex = 0;

        iconMenu.localPosition = new Vector3(-200f, buttons[currentIndex].localPosition.y-5f);

        if (menu.activeInHierarchy)
            buttons[currentIndex].GetComponent<AudioSource>().Play();
    }
    public void UpdateOptions()
    {
        if (currentIndex < 0)
            currentIndex = options.Length - 1;
        else if (currentIndex > options.Length - 1)
            currentIndex = 0;

        iconOption.localPosition = new Vector3(-200f, options[currentIndex].localPosition.y - 5f);

        switch (currentIndex)
        {
            case 0:
                mixer.GetFloat("masterVolume", out currentVolume);
                break;
            case 1:
                mixer.GetFloat("musicVolume", out currentVolume);
                break;
            case 2:
                mixer.GetFloat("sfxVolume", out currentVolume);
                break;
            case 3:
                mixer.GetFloat("vfxVolume", out currentVolume);
                break;
        }
        if(option.activeInHierarchy)
            options[currentIndex].GetComponent<AudioSource>().Play();
    }

    public void MenuButtonPress()
    {
        switch (currentIndex)
        {
            case 0:
                if (mainManu)
                    SceneManager.LoadScene("GameScene");
                else
                    GlobalUIInfo.i.ShowMenu(false);
                break;
            case 1:
                currentMenu = currentIndex;
                currentIndex = 0;
                UpdateOptions();

                option.SetActive(true);
                option.GetComponent<AudioSource>().Play();

                float val;
                mixer.GetFloat("masterVolume", out val);
                AdjustSlider(0, val);

                mixer.GetFloat("musicVolume", out val);
                AdjustSlider(1, val);

                mixer.GetFloat("sfxVolume", out val);
                AdjustSlider(2, val);

                mixer.GetFloat("vfxVolume", out val);
                AdjustSlider(3, val);

                menu.SetActive(false);
                option.SetActive(true);
                break;
            case 2:
                currentMenu = currentIndex;
                menu.SetActive(false);
                instructions.SetActive(true);
                instructions.GetComponent<AudioSource>().Play();

                break;
            case 3:
                if (mainManu)
                    Application.Quit();
                else
                    SceneManager.LoadScene("MenuScene");
                break;
        }
    }
    public void AdjustSlider(int slider, float value)
    {
        volumeSliders[slider].localScale = new Vector3((float)((int)((value + 80) / 10))/10, 0.7f,1);
    }
    public void InstructionsButtonPress()
    {
        currentMenu = 0;
        currentIndex = 2;
        menu.SetActive(true);
        UpdateMenu();
        instructions.SetActive(false);
    }
    public void OptionsButtonPress()
    {
        if (currentIndex == 4)
        {
            currentMenu = 0;
            currentIndex = 1;
            menu.SetActive(true);
            UpdateMenu();
            option.SetActive(false);
        }
    }
}