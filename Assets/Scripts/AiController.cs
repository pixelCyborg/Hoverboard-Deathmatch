using UnityEngine;
using System.Collections;

public class AiController : MonoBehaviour {
    MovementController movement;
    CombatController combat;

    private bool oldThrowWeapon;
    private bool throwWeapon;
    private float moveSpeed;
    public float turnSpeed;
    private bool dropWeapon;

    public Transform target;
    LayerMask mask;

    Collider[] nearbyPlayers;


    // Use this for initialization
    void Start()
    {
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
        mask = LayerMask.NameToLayer("Player");
        InvokeRepeating("TurnDecision", 0.2f, 2.0f);
        InvokeRepeating("CheckForTargets", 0.2f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (throwWeapon)
        {
            combat.Charge();
        }
        if (oldThrowWeapon == true && throwWeapon == false)
        {
            //Throw towards target
        }

        if (dropWeapon)
        {
            combat.Drop();
        }

        if(target)
        {
            if(Mathf.Abs(turnSpeed) < 1)
            {
                moveSpeed = 1;
            }
            else
            {
                moveSpeed = 0.5f;
            }

            Vector3 targetDir = target.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
        }
        else
        {
            moveSpeed = 1;
        }

        movement.turn = turnSpeed;
        movement.thrust = moveSpeed;

        oldThrowWeapon = throwWeapon;
    }

    void CheckForTargets()
    {
        if (target == null)
        {
            nearbyPlayers = Physics.OverlapSphere(transform.position, 25.0f, mask);
            if(nearbyPlayers != null && nearbyPlayers.Length > 0)
            {
                target = nearbyPlayers[Random.Range(0, nearbyPlayers.Length - 1)].transform;
                Debug.Log("Target found!");
            }
        }
    }

    void TurnDecision()
    {
        if(Physics.Raycast(transform.position, transform.forward, 25, ~mask))
        {
            turnSpeed = 1;
            moveSpeed = 0.5f;
        }
        else if (target == null)
        {
            float turnDirection = Random.Range(-100, 100);
            turnSpeed = turnDirection / 100.0f;
        }
    }
}
