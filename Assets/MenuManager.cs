using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MenuManager : MonoBehaviour {
    public static int[] players = new int[4] { -1, -1, -1, -1 };
    public static Color[] colors = new Color[4];

    void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    public static void AddPlayer(int playerNum, Color color)
    {
        players[playerNum - 1] = playerNum;
        colors[playerNum - 1] = color;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Starting the level!");
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        StartCoroutine(InitializeMap());
    }

    IEnumerator InitializeMap()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        FindObjectOfType<MapController>().Initialize(players, colors);
        SceneManager.UnloadScene(0);
    }
}
