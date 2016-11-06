using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {
    MovementController movement;
    CombatController combat;
    Camera myCamera;
    public Vector3 lookDirection;
    Vector3 target;
    private Transform aimReticle;
    private Renderer reticleRend;

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
    LayerMask obstacleMask;
    private bool charging = false;

    public Color playerColor;
    Text scoreText;

    Vector3 lastLookDirection;

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
        if(myCamera == null)
        {
            myCamera = Camera.main;
        }
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
        movement.controlType = controlType;
        obstacleMask = ~(1 << LayerMask.NameToLayer("Obstacle"));
        //obstacleMask = ~obstacleMask;
        aimReticle = transform.Find("AimReticle");
        reticleRend = aimReticle.GetComponent<Renderer>();

        //Set Colors
        //aimReticle.GetComponent<Renderer>().material.color = playerColor;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for(int i = 0; i < renderers.Length; i++)
        {
            if(renderers[i].gameObject == aimReticle.gameObject)
            {
                renderers[i].material.color = playerColor;
            }
            else
            {
                renderers[i].material.SetColor("_OutlineColor", playerColor);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        lookDirection = new Vector3(Input.GetAxis("LookHorizontal" + playerControlString), Input.GetAxis("LookVertical" + playerControlString), 0);

        if (Input.GetButtonDown("Boost" + playerControlString))
        {
            movement.boosting = true;
        }
        if (Input.GetButtonUp("Boost" + playerControlString))
        {
            movement.boosting = false;
        }

        if (Input.GetButtonDown("Drift" + playerControlString))
        {
            movement.drifting = true;
        }
        if (Input.GetButtonUp("Drift" + playerControlString))
        {
            movement.drifting = false;
        }
        //Keyboard Fire mechanism
        if(Input.GetButtonDown("Fire" + playerControlString))
        {
            if (controlType == ControlType.Keyboard)
            {
                combat.Charge();
            }
            else { 
                RaycastHit hit;
                target = Vector3.zero;
                Vector3 direction = new Vector3(lookDirection.x, 0, lookDirection.y) * 10.0f;
                Physics.Raycast(transform.position + direction + Vector3.up * 50, Vector3.down, out hit, 1000, obstacleMask);
                target = hit.point;
                target.y += 1;
                combat.Attack(target);
                charging = false;
            }
        }
        if(Input.GetButtonUp("Fire" + playerControlString))
        {
            if (controlType == ControlType.Keyboard)
            {
                RaycastHit hit;
                target = Vector3.zero;
                Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out hit, 1000, obstacleMask);
                Debug.Log(hit.point);
                target = hit.point;
                target.y += 1;
                //target.y = transform.position.y;
                combat.Attack(target);
            }
        }

        if (lookDirection.magnitude > 0 && !charging)
        {
            combat.Charge();
            charging = true;
        }
        else if(lookDirection.magnitude < 0.1f)
        {
            charging = false;
        }

        if(Input.GetButtonDown("Drop" + playerControlString))
        {
            combat.Drop();
        }

        movement.turn = Input.GetAxis("Horizontal" + playerControlString);
        movement.thrust = Input.GetAxis("Vertical" + playerControlString);

        if (lookDirection.magnitude > 0.5f)
        {
            Vector3 targetDir = Vector3.RotateTowards(aimReticle.forward, new Vector3(-lookDirection.x, 0, -lookDirection.y), 1.0f, 0.0f);
            aimReticle.rotation = Quaternion.LookRotation(targetDir);
            reticleRend.enabled = true;
        }
        else
        {
            reticleRend.enabled = false;
        }

        lastLookDirection = lookDirection;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 2.0f);
    }
}
