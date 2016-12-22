using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TeamUtility.IO;

public class PlayerController : MonoBehaviour {
    MovementController movement;
    CombatController combat;
    Camera myCamera;
    public Vector3 lookDirection;
    Vector3 target;
    private Transform aimReticle;
    private Renderer reticleRend;
    private ThirdPersonOrbitCam playerCam;
    private PauseMenu pauseMenu;

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
    private bool charging = false;
    private bool pushingStick = false;

    public Color playerColor;
    Text scoreText;

    Vector3 lastLookDirection;

	// Use this for initialization
	public void Initialize () {
        switch (player)
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
        pauseMenu = transform.parent.GetComponentInChildren<PauseMenu>();
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
        movement.controlType = controlType;
        aimReticle = transform.Find("AimReticle");
        reticleRend = aimReticle.GetComponent<Renderer>();
        playerCam = transform.parent.GetComponentInChildren<ThirdPersonOrbitCam>();

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
                renderers[i].material.color = playerColor;
            }
        }
        reticleRend.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (pauseMenu.paused) {
            float vertical = InputManager.GetAxis("Vertical" + playerControlString);
            movement.turn = 0;
            movement.thrust = 0;

            if(InputManager.GetButtonDown("Fire" + playerControlString))
            {
                pauseMenu.Select();
            }

            if(InputManager.GetButtonDown("Start" + playerControlString))
            {
                pauseMenu.Resume();
            }

            if(vertical > 0.9f)
            {
                if(!pushingStick)
                {
                    pushingStick = true;
                    pauseMenu.Up();
                }
            }
            else if(vertical < -0.9f)
            {
                if (!pushingStick)
                {
                    pushingStick = true;
                    pauseMenu.Down();
                }
            }
            else
            {
                pushingStick = false;
            }

            return;
        }
        lookDirection = new Vector3(InputManager.GetAxis("LookHorizontal" + playerControlString), InputManager.GetAxis("LookVertical" + playerControlString), 0);
        if(Mathf.Abs(lookDirection.x) < 0.2f)
        {
            lookDirection.x = 0;
        }
        if(Mathf.Abs(lookDirection.y) < 0.2f)
        {
            lookDirection.y = 0;
        }

        if (InputManager.GetButtonDown("Boost" + playerControlString))
        {
            playerCam.targetFOV = playerCam.defaultFOV * 1.5f;
            movement.boosting = true;
        }
        if (InputManager.GetButtonUp("Boost" + playerControlString))
        {
            playerCam.targetFOV = playerCam.defaultFOV;
            movement.boosting = false;
        }

        if (InputManager.GetButtonDown("Drift" + playerControlString))
        {
            movement.drifting = true;
        }
        if (InputManager.GetButtonUp("Drift" + playerControlString))
        {
            movement.drifting = false;
        }
        //Keyboard Fire mechanism
        if(InputManager.GetButtonDown("Fire" + playerControlString))
        {
            combat.Charge();
        }
        if(InputManager.GetButtonUp("Fire" + playerControlString))
        {
            {
                Vector3 target = Vector3.zero;
                Transform camTransform = myCamera.transform;
                target = camTransform.position + camTransform.forward * 100;
                combat.Attack(target);
            }
        }

        /*
        if (lookDirection.magnitude > 0 && !charging)
        {
            combat.Charge();
            charging = true;
        }
        else if(lookDirection.magnitude < 0.1f)
        {
            charging = false;
        }*/

        if(InputManager.GetButtonDown("Drop" + playerControlString))
        {
            combat.Drop();
        }

        if(InputManager.GetButtonDown("Start" + playerControlString)) {
            pauseMenu.Pause();
        }

        movement.turn = InputManager.GetAxis("Horizontal" + playerControlString);
        movement.thrust = InputManager.GetAxis("Vertical" + playerControlString);

        /*if (lookDirection.magnitude > 0.5f)
        {
            Vector3 targetDir = Vector3.RotateTowards(aimReticle.forward, new Vector3(-lookDirection.x, 0, -lookDirection.y), 1.0f, 0.0f);
            aimReticle.rotation = Quaternion.LookRotation(targetDir);
            reticleRend.enabled = true;
        }
        else
        {
            reticleRend.enabled = false;
        }*/

        playerCam.LookUpdate(lookDirection);
        lastLookDirection = lookDirection;
    }

    void LateUpdate()
    {
        //playerCam.LookUpdate(lookDirection);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 2.0f);
    }
}
