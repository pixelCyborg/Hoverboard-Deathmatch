using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using TeamUtility.IO;

public class MenuManager : MonoBehaviour {
    public static List<int> players = new List<int>();
    public static List<Color> colors = new List<Color>();
    public static List<Player> playerObjs = new List<Player>();
    ModeSelector selector;

    public GameObject mainScreen;
    public GameObject controlScreen;

    void Start()
    {
        selector = FindObjectOfType<ModeSelector>();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        GoToMain();
    }

    void Update()
    {
        /*foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log(kcode);
        }*/

        for (int i = 1; i < 5; i++)
        {
            if (InputManager.GetButtonDown("Drift PLAYER_" + i))
            {
                selector.Next();
            }
            else if (InputManager.GetButtonDown("Fire PLAYER_" + i))
            {
                selector.Previous();
            }
        }
    }

    public void GoToMain()
    {
        mainScreen.SetActive(true);
        controlScreen.SetActive(false);
    }

    public void GoToControls()
    {
        mainScreen.SetActive(false);
        controlScreen.SetActive(true);
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
        Goal.mode = ModeSelector.gameMode;
        if(Goal.mode == Goal.GameMode.Deathmatch)
        {
            FindObjectOfType<Goal>().gameObject.SetActive(false);
        }
        SceneManager.UnloadScene(0);
    }
}
