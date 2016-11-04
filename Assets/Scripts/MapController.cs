using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
    public static Vector2 bounds;
    public static GameObject[] spawnPoints;
    public static List<Vector3> weaponSpawns = new List<Vector3>();
    public static List<Vector3> ballSpawns = new List<Vector3>();
    private static Transform ball;

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
}
