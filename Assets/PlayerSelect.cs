using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Networking;

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

    void Start()
    {
        selected.SetActive(false);
        controlString = " PLAYER_" + number;
    }

    void Update()
    {
        if (Input.GetButtonDown("Boost" + controlString))
        {
            if (!added)
            {
                AddPlayer(); 
                ColorSelecter();
            }
        }

        if(added)
        {
            float x = Input.GetAxis("Horizontal" + controlString);
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

            if (Input.GetButtonDown("Start" + controlString))
            {
                FindObjectOfType<MenuManager>().StartGame();
            }
        }
    }

    public void AddPlayer()
    {
        if (!added)
        {
            //GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            color = GetComponentInChildren<Text>().color;
            MenuManager.AddPlayer(number, color);
            selected.SetActive(true);
            notSelected.SetActive(false);
            added = true;
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

    public void SelectColor(Color _color)
    {
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
