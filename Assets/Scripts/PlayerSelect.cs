using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Networking;
using TeamUtility.IO;

public class PlayerSelect : MonoBehaviour {
    public int number;
    public Color color = Color.white;
    public GameObject selected;
    public GameObject notSelected;
    private bool added = false;
    private string controlString;

    List<RectTransform> colors;
    int currentColor;
    int oldColor;
    bool changing;

    public AudioClip moveSound;
    public AudioClip selectSound;
    private AudioSource source;

    void Start()
    {
        selected.SetActive(false);
        controlString = " PLAYER_" + number;
        source = GetComponent<AudioSource>();
    }

    void PlayMove()
    {
        source.clip = moveSound;
        source.Play();
    }

    void PlaySelect()
    {
        source.clip = selectSound;
        source.Play();
    }

    void Update()
    {
        if(InputManager.GetButtonDown("Drop" + controlString))
        {
            FindObjectOfType<MenuManager>().GoToControls();
        }

        if (InputManager.GetButtonDown("Fire" + controlString))
        {
            if (!added)
            {
                AddPlayer();
            }
        }

        if(added)
        {
            float x = InputManager.GetAxis("Horizontal" + controlString);
            if (!changing)
            {
                if (x > 0.9f)
                {
                    NextColor();
                    changing = true;
                }
                else if (x < -0.9f)
                {
                    PrevColor();
                    changing = true;
                }
            }
            else
            {
                if(x < 0.1f && x > -0.1f)
                {
                    changing = false;
                }
            }

            if (InputManager.GetButtonDown("Start" + controlString))
            {
                PlaySelect();
                FindObjectOfType<MenuManager>().StartGame();
            }
        }
    }

    public void AddPlayer()
    {
        if (!added)
        {
            PlaySelect();
            //GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            color = GetComponentInChildren<Text>().color;
            MenuManager.AddPlayer(number, color);
            selected.SetActive(true);
            notSelected.SetActive(false);
            added = true;
            ColorSelecter();
        }
    }

    private void ColorSelecter()
    {
        colors = new List<RectTransform>();
        Transform colorController = transform.Find("Selected"); //transform.Find("Selected").GetComponentsInChildren<RectTransform>();
        foreach(Transform child in colorController)
        {
            colors.Add(child.GetComponent<RectTransform>());
        }
        currentColor = 0;
        colors[0].DOScale(1.2f, 0.5f);
    }

    void NextColor()
    {
        if (currentColor < colors.Count - 1)
        {
            oldColor = currentColor;
            colors[oldColor].DOScale(1.0f, 0.5f);
            currentColor++;
            colors[currentColor].DOScale(1.2f, 0.5f);
            SelectColor(colors[currentColor].GetComponent<Image>().color);
        }
    }

    void PrevColor()
    {
        if (currentColor > 0)
        {
            oldColor = currentColor;
            colors[oldColor].DOScale(1.0f, 0.5f);
            currentColor--;
            colors[currentColor].DOScale(1.2f, 0.5f);
            SelectColor(colors[currentColor].GetComponent<Image>().color);
        }
    }

    public void SelectColor(Color _color, Transform selectBox)
    {
        oldColor = currentColor;
        colors[oldColor].DOScale(1.0f, 0.5f);
        currentColor = selectBox.GetSiblingIndex();
        colors[currentColor].DOScale(1.2f, 0.5f);
        SelectColor(colors[currentColor].GetComponent<Image>().color);

        SelectColor(_color);
    }

    public void SelectColor(Color _color)
    {
        PlayMove();
        color = _color;
        Text[] text = GetComponentsInChildren<Text>();
        for(int i = 0; i < text.Length; i++)
        {
            text[i].color = color;
        }
        
        for(int i = 0; i < MenuManager.colors.Count; i++)
        {
            if(MenuManager.players[i] == number)
            {
                MenuManager.colors[i] = color;
            }
        }
    }
}
