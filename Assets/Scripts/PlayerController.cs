using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    MovementController movement;
    CombatController combat;
    Camera myCamera;
    public Vector3 lookDirection;
    Vector3 target;

    public enum ControlType
    {
        Keyboard, Controller
    }
    public enum Player
    {
        One, Two, Three, Four, Debug
    }
    public ControlType controlType;
    public Player player;
    private string playerControlString = "";

	// Use this for initialization
	void Start () {
        switch(player)
        {
            case Player.One:
                playerControlString = " PLAYER_1";
                break;
            case Player.Two:
                playerControlString = " PLAYER_2";
                break;
            case Player.Three:
                playerControlString = " PLAYER_3";
                break;
            case Player.Four:
                playerControlString = " PLAYER_4";
                break;
            case Player.Debug:
                break;
        }
        myCamera = transform.parent.GetComponentInChildren<Camera>();
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
	}
	
	// Update is called once per frame
	void Update () {
        lookDirection = new Vector3(Input.GetAxis("LookHorizontal" + playerControlString), Input.GetAxis("LookVertical" + playerControlString), 0);

        if (Input.GetButtonDown("Boost" + playerControlString))
        {
            Debug.Log("Boosting?");
            movement.boosting = true;
        }
        if (Input.GetButtonUp("Boost" + playerControlString))
        {
            movement.boosting = false;
        }

        if (!movement.boosting && Input.GetButtonDown("Drift" + playerControlString))
        {
            Debug.Log("Drifting");
            movement.drifting = true;
        }
        if (Input.GetButtonUp("Drift" + playerControlString))
        {
            movement.drifting = false;
        }

        if(Input.GetButtonDown("Fire" + playerControlString))
        {
            combat.Charge();
        }
        if(Input.GetButtonUp("Fire" + playerControlString))
        {
            RaycastHit hit;
            target = Vector3.zero;
            if (controlType == ControlType.Keyboard)
            {
                Physics.Raycast(myCamera.ViewportPointToRay(myCamera.ScreenToViewportPoint(Input.mousePosition)), out hit);
                target = hit.point;
                target.y = transform.position.y;
            }
            else
            {
                Vector3 viewportDirection = lookDirection + new Vector3(1, 1, 0);
                viewportDirection *= 0.5f;
                Debug.Log(viewportDirection);
                Physics.Raycast(myCamera.ViewportPointToRay(viewportDirection), out hit);
                target = hit.point;
                target.y = transform.position.y;
            }

            combat.Attack(target);
        }

        if(Input.GetButtonDown("Drop" + playerControlString))
        {
            combat.Drop();
        }

        movement.turn = Input.GetAxis("Horizontal" + playerControlString);
        movement.thrust = Input.GetAxis("Vertical" + playerControlString);

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 0.25f);
    }
}
