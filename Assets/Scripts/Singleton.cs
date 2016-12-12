using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour {
    void Awake()
    {
        if(FindObjectsOfType<Singleton>().Length > 1)
        {
            Destroy(this);
        }
    }
}
