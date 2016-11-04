using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Goal : MonoBehaviour {
    int[] scores;
    public Transform scoreBoard;
    Text[] scoreBoards;


	// Use this for initialization
	void Start () {
        scores = new int[4] { 0, 0, 0, 0 };
        scoreBoards = new Text[4];
        for(int i = 0; i < scoreBoard.childCount; i++)
        {
            scoreBoards[i] = scoreBoard.GetChild(i).GetComponent<Text>();
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            CombatController combat = col.GetComponent<CombatController>();
            if(combat && combat.equippedWeapon && combat.equippedWeapon.type == Weapon.WeaponType.Ball)
            {
                Score(combat);
            }
        }
    }
	
	public void Score(CombatController combat)
    {
        int playerNum = -1;
        PlayerController player = combat.GetComponent<PlayerController>();
        if (player) {
            switch (player.player)
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
        }
        scores[playerNum]++;
        scoreBoards[playerNum].text = scores[playerNum].ToString();
        GameObject ball = combat.equippedWeapon.gameObject;
        combat.Drop();
        ball.SetActive(false);
        MapController.SpawnBall();
    }
}
