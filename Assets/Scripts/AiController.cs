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
    LayerMask obstacleMask;

    Collider[] nearbyPlayers;
    bool turning;


    // Use this for initialization
    void Start()
    {
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
        mask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Terrain");
        turnSpeed = 0;
        turning = false;
        //InvokeRepeating("TurnDecision", 0.2f, 2.0f);
        //InvokeRepeating("CheckForTargets", 0.2f, 2.0f);
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

        {
            RaycastHit hit;
            if(!turning)
            {
                moveSpeed = 0.8f;
                turnSpeed = 0;
                if (Physics.SphereCast(transform.position + Vector3.up * 4, 1.0f, transform.forward, out hit, 5.0f, obstacleMask))
                {
                    Debug.Log(hit.transform.name);
                    Debug.Log("Theres something in front of me");
                    StartCoroutine(Avoidance());
                }
            }
            else
            {
                moveSpeed = 0.2f;
            }
        }

        movement.turn = turnSpeed;
        movement.thrust = moveSpeed;

        oldThrowWeapon = throwWeapon;
    }

    IEnumerator Avoidance()
    {
        turning = true;
        turnSpeed = Random.Range(60, 80);
        turnSpeed *= Random.Range(0, 1) == 1 ? -1 : 1;
        turnSpeed *= 0.01f;
        yield return new WaitForSeconds(1.0f);
        turning = false;
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
        if (target == null)
        {
            float turnDirection = Random.Range(-100, 100);
            turnSpeed = turnDirection / 100.0f;
        }
    }
}
