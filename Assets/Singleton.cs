using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour {
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
}
