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
    public ControlConfig[] configs;

    public AudioClip moveSound;
    public AudioClip selectSound;
    private AudioSource source;

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

    void Start()
    {
        source = GetComponent<AudioSource>();
        InputManager.Load();
        selector = FindObjectOfType<ModeSelector>();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        GoToMain();
    }

    void Update()
    {
        if(mainScreen.gameObject.activeSelf) {
            for (int i = 1; i < 5; i++)
            {
                if (!configs[i - 1].receivingInput)
                {
                    if (InputManager.GetButtonDown("Drift PLAYER_" + i))
                    {
                        selector.Next();
                    }
                }
            }
        }
    }

    public void GoToMain()
    {
        if(controlScreen.activeSelf)
        {
            InputManager.Save();
        }

        mainScreen.SetActive(true);
        controlScreen.SetActive(false);
        PlaySelect();
    }

    public void GoToControls()
    {
        mainScreen.SetActive(false);
        controlScreen.SetActive(true);
        PlaySelect();
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
        if (arg0.buildIndex != 0)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            StartCoroutine(InitializeMap());
        }
        else
        {
            Destroy(this);
        }
    }

    IEnumerator InitializeMap()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        FindObjectOfType<MapController>().Initialize(players.ToArray(), colors.ToArray());
        Goal.mode = ModeSelector.gameMode;
        if(Goal.mode == Goal.GameMode.Deathmatch)
        {
            Transform goal = FindObjectOfType<Goal>().transform;
            goal.GetComponent<Collider>().enabled = false;
            goal.GetComponent<Renderer>().enabled = false;
        }
        SceneManager.UnloadScene(0);
    }
}
