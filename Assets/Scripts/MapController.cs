using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
    public static Vector2 bounds;
    public static GameObject[] spawnPoints;
    private static GameObject spearPrefab;
    //List<Weapon> spear;
    GameObject[] player;

    void Start()
    {
        spearPrefab = Resources.Load("Spear") as GameObject;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
    }

    public static Vector3 RandomSpawnPosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform.position;
    }

    public static void SpawnSpear()
    {
        Instantiate(spearPrefab, new Vector3(Random.Range(-bounds.x, bounds.x), 2f, Random.Range(-bounds.y, bounds.y)), Random.rotation);
    }
}
