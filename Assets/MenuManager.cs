using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MenuManager : MonoBehaviour {
    public static List<int> players = new List<int>();
    public static List<Color> colors = new List<Color>();
    public static List<Player> playerObjs = new List<Player>();

    void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    void Update()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
    }

    public static void AddPlayer(int playerNum, Color color)
    {
        players.Add(playerNum);
        colors.Add(color);
        playerObjs.Add(new Player(playerNum, color));
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        StartCoroutine(InitializeMap());
    }

    IEnumerator InitializeMap()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        FindObjectOfType<MapController>().Initialize(players.ToArray(), colors.ToArray());
        SceneManager.UnloadScene(0);
    }
}
