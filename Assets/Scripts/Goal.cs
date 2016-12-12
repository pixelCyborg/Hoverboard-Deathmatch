using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {
    int[] scores;
    public Transform scoreBoard;
    public Text[] scoreBoards;
    ParticleSystem particles;
    public Image winImage;
    public int winScore = 5;
    public enum GameMode { Oddball, Deathmatch, CTF };
    public static GameMode mode;

	// Use this for initialization
	void Start () {

        scores = new int[4] { 0, 0, 0, 0 };
        /*scoreBoards = new Text[4];
        for(int i = 0; i < scoreBoard.childCount; i++)
        {
            scoreBoards[i] = scoreBoard.GetChild(i).GetComponent<Text>();
        }*/

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < 4; i++)
        {
            if (i < players.Length)
            {
                int playerNum = GetPlayerNum(players[i].player);
                scoreBoards[playerNum].color = players[i].playerColor;
            }
            else
            {
                Destroy(scoreBoards[i].gameObject);
            }
        }
        winImage.color = Color.clear;
        winImage.gameObject.SetActive(false);

        particles = GetComponentInChildren<ParticleSystem>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (mode == GameMode.Oddball)
        {
            if (col.tag == "Player")
            {
                CombatController combat = col.GetComponent<CombatController>();
                if (combat && combat.equippedWeapon && combat.equippedWeapon.type == Weapon.WeaponType.Ball)
                {
                    Score(combat);
                }
            }
        }
    }
	
	public void Score(CombatController combat)
    {
        int playerNum = -1;
        PlayerController player = combat.GetComponent<PlayerController>();
        if(player) {
            playerNum = GetPlayerNum(player.player); 
        }
        scores[playerNum]++;
        scoreBoards[playerNum].text = scores[playerNum].ToString();
        if(scores[playerNum] >= winScore)
        {
            Win(combat);
        }

        if(mode == GameMode.Oddball)
        {
            GameObject ball = combat.equippedWeapon.gameObject;
            combat.Drop();
            ball.SetActive(false);
            StartCoroutine(DelayedEmit(combat));
            MapController.SpawnBall();
        }
    }


    void Win(CombatController combat)
    {
        int playerNum = GetPlayerNum(combat.GetComponent<PlayerController>().player);
        winImage.gameObject.SetActive(true);
        winImage.GetComponentInChildren<Text>().color = combat.GetComponent<PlayerController>().playerColor;
        winImage.GetComponentInChildren<Text>().text = "Player " + (playerNum+1) + " Wins!";
        winImage.DOFade(1.0f, 5.0f).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        
    }

    IEnumerator DelayedEmit(CombatController combat)
    {
        yield return new WaitForEndOfFrame();
        particles.transform.position = combat.transform.position;
        particles.Emit(5);
    }

    int GetPlayerNum(PlayerController.Player player)
    {
        int playerNum = -1;
        switch (player)
        {
            case PlayerController.Player.Debug:
            case PlayerController.Player.One:
                playerNum = 0;
                break;
            case PlayerController.Player.Two:
                playerNum = 1;
                break;
            case PlayerController.Player.Three:
                playerNum = 2;
                break;
            case PlayerController.Player.Four:
                playerNum = 3;
                break;
        }
        return playerNum;
    }
}
