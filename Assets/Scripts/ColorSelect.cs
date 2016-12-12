using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorSelect : MonoBehaviour {
    public void Select()
    {
        GetComponentInParent<PlayerSelect>().SelectColor(GetComponent<Image>().color);
    }
}
