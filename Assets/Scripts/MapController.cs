using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
    public static Vector2 bounds;
    public static GameObject[] spawnPoints;
    public static List<Vector3> weaponSpawns = new List<Vector3>();
    public static List<Vector3> ballSpawns = new List<Vector3>();
    private static Transform ball;

    public GameObject playerPrefab;
    //public Transform Characters;

    private static GameObject spearPrefab;
    //List<Weapon> spear;
    GameObject[] player;

    void Start()
    {
        spearPrefab = Resources.Load("Spear") as GameObject;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        Weapon[] weapons = FindObjectsOfType<Weapon>();
        for(int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type != Weapon.WeaponType.Ball) weaponSpawns.Add(weapons[i].transform.position);
            else {
                ballSpawns.Add(weapons[i].transform.position);
                ball = weapons[i].transform;
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Initialize(4, new Color[4]);
    }

    public static Vector3 RandomSpawnPosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
    }

    public static void SpawnSpear()
    {
        Instantiate(spearPrefab, weaponSpawns[Random.Range(0, weaponSpawns.Count - 1)], Random.rotation);
    }

    public static void SpawnBall()
    {
        ball.position = ballSpawns[Random.Range(0, ballSpawns.Count - 1)];
        ball.gameObject.SetActive(true);
    }

    public void Initialize(int[] players, Color[] colors)
    {
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != -1)
            {
                GameObject player = Instantiate(playerPrefab);
                SceneManager.MoveGameObjectToScene(player, gameObject.scene);
                PlayerController control = player.GetComponentInChildren<PlayerController>();
                Camera playerCam = player.GetComponentInChildren<Camera>();

                player.transform.position = spawnPoints[i].transform.position;
                control.controlType = PlayerController.ControlType.Controller;

                if (players[i] == 1)
                {
                    control.player = PlayerController.Player.One;
                    playerCam.rect = GetCameraDimensions(1, players.Length);
                }
                else if (players[i] == 2)
                {
                    control.player = PlayerController.Player.Two;
                    playerCam.rect = GetCameraDimensions(2, players.Length);
                }
                else if (players[i] == 3)
                {
                    control.player = PlayerController.Player.Three;
                    playerCam.rect = GetCameraDimensions(3, players.Length);
                }
                else if (players[i] == 4)
                {
                    control.player = PlayerController.Player.Four;
                    playerCam.rect = GetCameraDimensions(4, players.Length);
                }
                control.playerColor = colors[i];
                control.Initialize();
            }
        }
    }

    Rect GetCameraDimensions(int playerNum, int playerTotal)
    {
        Rect rect = new Rect();
        if(playerNum == 1)
        {
            if(playerTotal == 1)
            {
                rect = new Rect(0, 0, 1, 1);
            }
            else if(playerTotal == 2 || playerTotal == 3)
            {
                rect = new Rect(0, 0.5f, 1, 0.5f);
            }
            else if(playerTotal == 4)
            {
                rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            }
        }
        else if(playerNum == 2)
        {
            if(playerTotal == 2)
            {
                rect = new Rect(0, 0, 1, 0.5f);
            }
            else if(playerTotal == 3)
            {
                rect = new Rect(0, 0, 0.5f, 0.5f);
            }
            else if (playerTotal == 4)
            {
                rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        else if(playerNum == 3)
        {
            if(playerTotal == 3)
            {
                rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
            }
            else if(playerTotal == 4)
            {
                rect = new Rect(0, 0f, 0.5f, 0.5f);
            }
        }
        else if(playerNum == 4)
        {
            rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
        return rect;
    }
}
