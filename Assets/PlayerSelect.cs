using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSelect : MonoBehaviour {
    public int number;
    Color color = Color.white;
    public GameObject selected;
    public GameObject notSelected;

    void Start()
    {
        selected.SetActive(false);
    }

    public void AddPlayer()
    {
        MenuManager.AddPlayer(number, color);
        selected.SetActive(true);
        notSelected.SetActive(false);
    }

    public void SelectColor(Color _color)
    {
        color = _color;
        Text[] text = GetComponentsInChildren<Text>();
        for(int i = 0; i < text.Length; i++)
        {
            text[i].color = color;
        }
    }
}
